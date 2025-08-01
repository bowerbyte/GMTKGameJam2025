using System;
using System.Collections.Generic;
using System.Linq;
using Project.Entities;
using Project.Entities.Requests;
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
                    if (UnityEngine.Random.value > 0.1f)
                    {
                        _tilesTypes[xi, zi] = TileType.Grass;   
                    }
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

        public LevelEntity AddEntityOfTypeAt(EntityType entityType, TileLocation pos)
        {
            var entity = LevelEntity.Create(entityType, this);
            this.AddEntityAt(entity, pos);
            return entity;
        }
        
        public void AddEntityAt(LevelEntity entity, TileLocation pos)
        {
            if (this.TryGetEntityAt(pos, out LevelEntity _))
            {
                throw new ArgumentException($"Position {pos} already contains an entity");
            }
            
            var localPos = LocationToBasePosition(pos);
            entity.SetLocalPositionImmediate(localPos);
            entity.Location = pos;
            
            _locationToEntity.Add(pos, entity);
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

        private void OnEntityMoved(TileLocation source, TileLocation destination)
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
            
            var moveRequests = actionRequests.OfType<MoveAction>().ToList();
            
            
            // Reconcile movement

            // TODO: Start with initial data about what positions are blocked/stepable
            var obstructions = GetObstructionMask();
            
            foreach (var request in moveRequests)
            {
                if (!LocationInBounds(request.destination)) { continue; }
                
                // TODO: Handle two entities facing each other and trying to move past each other
                // obstructions[request.source.x, request.source.z] -= 1; // allow entities to move into a tile as an existing entity moves out
                obstructions[request.destination.x, request.destination.z] += 1;
            }
            
            foreach (var request in moveRequests)
            {
                bool success = false;

                if (LocationInBounds(request.destination))
                {
                    int count = obstructions[request.destination.x, request.destination.z];
                    success = count <= 1;   
                }
                
                request.Actor.OnActionBegin(request, success);
                if (success)
                {
                    OnEntityMoved(request.source, request.destination);
                }
                
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