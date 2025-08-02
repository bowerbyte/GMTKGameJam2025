using System;
using System.Collections.Generic;
using System.Linq;
using Project.Entities;
using Project.Entities.Actions;
using Project.Enums;
using Project.Level.Settings;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Level
{
    public class LevelManager : MonoBehaviour
    {
        public int2 GridSize => _settings.gridDimensions;
        
        private LevelManagerSettings _settings;
        
        private TileObject[,] _tileObjects;

        private Dictionary<TileType, GameObject> _tilePrefabMap;
        
        
        private TileType[,] _tilesTypes;
        private Dictionary<TileLocation, LevelEntity> _locationToEntity; // TODO: For some entities to not be obstructing, should this be a list?
        private List<LevelEntity> _entities;
        
        
        public void Initialize(LevelManagerSettings settings)
        {
            InitializeTilePrefabMap();

            ResetForLevelSettings(settings);
            
            _entities = new ();
        }

        private void ResetForLevelSettings(LevelManagerSettings settings)
        {
            _settings = settings;
            _tilesTypes = new TileType[this.GridSize[0], this.GridSize[1]];
            _locationToEntity = new Dictionary<TileLocation, LevelEntity>();
            _entities = new List<LevelEntity>();

            for (int xi = 0; xi < this.GridSize[0]; xi++)
            {
                for (int zi = 0; zi < this.GridSize[1]; zi++)
                {
                    _tilesTypes[xi, zi] = TileType.Grass;
                }
            }

            // create tile objects

            _tileObjects = new TileObject[this.GridSize[0], this.GridSize[1]];
            
            for (int xi = 0; xi < this.GridSize[0]; xi++)
            {
                for (int zi = 0; zi < this.GridSize[1]; zi++)
                {
                    var tileType = _tilesTypes[xi, zi];
                    if (tileType == TileType.Empty) { continue; }
                    
                    _tileObjects[xi, zi] = GetTileObjectForType(tileType);

                    var tilePos = new TileLocation(xi, zi);
                    
                    _tileObjects[xi, zi].transform.position = LocationToBasePosition(tilePos);
                }
            }
        }

        private TileObject GetTileObjectForType(TileType tileType)
        {
            var prefab = _tilePrefabMap[tileType];
            var go = Instantiate(prefab);
            return go.GetComponent<TileObject>();
        }

        private void InitializeTilePrefabMap()
        {
            _tilePrefabMap = new Dictionary<TileType, GameObject>();
            var tileTypes = new List<TileType>()
            {
                TileType.Grass
            };
            foreach (var type in tileTypes)
            {
                GameObject prefab = Resources.Load<GameObject>($"Prefabs/Tiles/Tile ({type})");
                _tilePrefabMap.Add(type, prefab);
            }
        }

        public float3 LocationToBasePosition(TileLocation tilePos)
        {
            var dims = _settings.gridDimensions;
            var maxTile = (float2)dims / 2f - 0.5f;
            var minTile = -maxTile;

            float2 tilePercentage = (int2)tilePos / (float2)(dims - 1);
            var baseWorldPos = math.lerp(minTile, maxTile, tilePercentage);
            return new float3(baseWorldPos.x, 0f, baseWorldPos.y);
        }

        public LevelEntity AddEntityOfTypeAt(EntityType entityType, TileLocation loc)
        {
            var entity = LevelEntity.Create(entityType, this);
            this.AddEntityAt(entity, loc);
            return entity;
        }

        public TileType GetTileType(TileLocation loc)
        {
            return _tilesTypes[loc.x, loc.z];
        }
        
        public void AddEntityAt(LevelEntity entity, TileLocation loc)
        {
            if (this.TryGetEntityAt(loc, out LevelEntity _))
            {
                throw new ArgumentException($"Position {loc} already contains an entity");
            }
            
            var localPos = LocationToBasePosition(loc);
            entity.SetLocalPositionImmediate(localPos);
            entity.Location = loc;
            
            _locationToEntity.Add(loc, entity);
            _entities.Add(entity);
        }

        public bool TryGetEntityAt(TileLocation pos, out LevelEntity levelEntity)
        {
            if (_locationToEntity.TryGetValue(pos, out levelEntity))
            {
                return true;
            }

            return false;
        }

        public void OnEntityMoved(TileLocation source, TileLocation destination)
        {
            if (!TryGetEntityAt(source, out LevelEntity entity))
            {
                throw new Exception($"No entity found at {source}");
            }
            if (TryGetEntityAt(destination, out LevelEntity dentity))
            {
                if (EntityTypes.HasFlag(dentity.Type, EntityFlags.Obstructing))
                {
                    throw new Exception($"Attempting to move entity to location {destination}, which already has a obstructing entity");   
                }
            }

            _locationToEntity.Remove(source);
            _locationToEntity.Add(destination, entity);
        }

        public bool LocationInBounds(TileLocation location)
        {
            return location.x >= 0 && location.z >= 0 && location.x < this.GridSize[0] && location.z < this.GridSize[1];
        }

        public void RunStep()
        {
            // TODO: Rewrite entity request system to let each entity be able to make a request, which could be a movement or interaction.
            //       Then reconcile all the requests, which can either succeed or fail.

            var actionRequests = new List<EntityAction>();

            foreach (var entity in _entities)
            {
                var request = entity.GetActionRequest();
                if (request != null)
                {
                    actionRequests.Add(request);
                }
            }
            
            var actionResults = new Dictionary<EntityAction, bool>();
            
            // Reconcile Movement
            
            var moveRequests = actionRequests.OfType<MoveAction>().ToList();

            var obstructions = GetObstructionMask();
            
            foreach (var request in moveRequests)
            {
                if (!LocationInBounds(request.destination)) { continue; }
                
                // TODO: Handle two entities facing each other and trying to move past each other
                // obstructions[request.source.x, request.source.z] -= 1; // allow entities to move into a tile as an existing entity moves out
                obstructions[request.destination.x, request.destination.z] += 1;
            }

            var postMovementEntityLocations = _locationToEntity.ToDictionary(
                entry => entry.Key,
                entry => entry.Value
            ); // copy
            
            foreach (var request in moveRequests)
            {
                bool success = false;

                if (LocationInBounds(request.destination))
                {
                    int count = obstructions[request.destination.x, request.destination.z];
                    success = count <= 1;   
                }
                
                actionResults.Add(request, success);

                if (success)
                {
                    postMovementEntityLocations.Remove(request.source);
                    postMovementEntityLocations.Add(request.destination, request.Actor);
                }
                
            }
            
            // Reconcile Harvesting
            
            var harvestCounts = new Dictionary<LevelEntity, int>();
            
            var harvestRequests = actionRequests.OfType<HarvestAction>().ToList();
            
            foreach (var request in harvestRequests)
            {
                if (!TryGetEntityAt(request.targetLocation, out LevelEntity targetEntity))
                {
                    continue;
                }
                harvestCounts.TryAdd(targetEntity, 0);
                harvestCounts[targetEntity] += 1;
            }
            
            foreach (var request in harvestRequests)
            {
                // check if entity exists at the target location
                if (!postMovementEntityLocations.TryGetValue(request.targetLocation, out LevelEntity targetEntity))
                {
                    actionResults.Add(request, false);
                    continue;
                }
                
                // check if entity can be harvested and if there's enough items to be harvested by ALL harvesting entities
                if (!targetEntity.Harvestable || targetEntity.AvailableItemCount() < harvestCounts[targetEntity])
                {
                    actionResults.Add(request, false);
                    continue;
                }
                
                actionResults.Add(request, true);
            }
            
            // Reconcile Deposits
            
            var depositCounts = new Dictionary<LevelEntity, int>();
            var depositRequests = actionRequests.OfType<DepositAction>().ToList();
            
            foreach (var request in depositRequests)
            {
                if (!TryGetEntityAt(request.targetLocation, out LevelEntity targetEntity))
                {
                    continue;
                }
                depositCounts.TryAdd(targetEntity, 0);
                depositCounts[targetEntity] += 1;
            }
            
            foreach (var request in depositRequests)
            {
                // check if entity exists at the target location
                if (!postMovementEntityLocations.TryGetValue(request.targetLocation, out LevelEntity targetEntity))
                {
                    actionResults.Add(request, false);
                    continue;
                }
                
                // check if entity can be deposited onto and if there's enough open item slots to be deposited into by ALL depositing entities
                if (!targetEntity.Depositable || targetEntity.AvailableItemSlotCount() < depositCounts[targetEntity])
                {
                    actionResults.Add(request, false);
                    continue;
                }
                
                actionResults.Add(request, true);
            }

            foreach (var action in actionResults.Keys)
            {
                action.Actor.OnActionBegin(action, actionResults[action]);
            }
        }

        public int[,] GetObstructionMask()
        {
            var mask = new int[this.GridSize[0], this.GridSize[1]];

            for (int xi = 0; xi < this.GridSize[0]; xi++)
            {
                for (int zi = 0; zi < this.GridSize[1]; zi++)
                {
                    var tile = _tilesTypes[xi, zi];
                    if (!TileTypes.HasFlag(tile, TileFlags.Walkable))
                    {
                        mask[xi, zi] = 100000;
                    }
                }
            }
            
            for (int xi = 0; xi < this.GridSize[0]; xi++)
            {
                for (int zi = 0; zi < this.GridSize[1]; zi++)
                {
                    var location = new TileLocation(xi, zi);
                    if (TryGetEntityAt(location, out LevelEntity entity))
                    {
                        if (EntityTypes.HasFlag(entity.Type, EntityFlags.Obstructing))
                        {
                            mask[xi, zi] += 1;
                        }
                    }
                }
            }

            return mask;
        }

        


        public static LevelManager Create(LevelManagerSettings settings)
        {
            var go = new GameObject("Level Manager");
            var levelManager = go.AddComponent<LevelManager>();
            levelManager.Initialize(settings);
            return levelManager;
        }
    }
}