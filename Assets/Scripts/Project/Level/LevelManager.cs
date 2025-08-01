using System;
using System.Collections.Generic;
using System.Linq;
using Project.Level.Enums;
using Project.Entities;
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
        private Dictionary<TilePosition, LevelEntity> _positionToEntity;
        private List<LevelEntity> _entities;

        public List<IMovableEntity> GetMovableEntities()
        {
            return _entities.OfType<IMovableEntity>().ToList();
        }
        
        
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
            _positionToEntity = new Dictionary<TilePosition, LevelEntity>();
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
                    _tileObjects[xi, zi] = GetTileObjectForType(_tilesTypes[xi, zi]);

                    var tilePos = new TilePosition(xi, zi);
                    
                    _tileObjects[xi, zi].transform.position = TilePositionToBaseLocalPosition(tilePos);
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

        public float3 TilePositionToBaseLocalPosition(TilePosition tilePos)
        {
            var dims = _settings.gridDimensions;
            var maxTile = (float2)dims / 2f - 0.5f;
            var minTile = -maxTile;

            float2 tilePercentage = (int2)tilePos / (float2)(dims - 1);
            var baseWorldPos = math.lerp(minTile, maxTile, tilePercentage);
            return new float3(baseWorldPos.x, 0f, baseWorldPos.y);
        }

        public void AddEntityOfTypeAt(EntityType entityType, TilePosition pos)
        {
            var entity = LevelEntity.Create(entityType, this);
            this.AddEntityAt(entity, pos);
        }
        
        public void AddEntityAt(LevelEntity entity, TilePosition pos)
        {
            if (this.TryGetEntityAt(pos, out LevelEntity _))
            {
                throw new ArgumentException($"Position {pos} already contains an entity");
            }
            
            var localPos = TilePositionToBaseLocalPosition(pos);
            entity.SetLocalPositionImmediate(localPos);
            entity.Position = pos;
            
            _positionToEntity.Add(pos, entity);
            _entities.Add(entity);
        }

        public bool TryGetEntityAt(TilePosition pos, out LevelEntity levelEntity)
        {
            if (_positionToEntity.TryGetValue(pos, out levelEntity))
            {
                return true;
            }

            return false;
        }

        public void RunStep()
        {
            // Compile move requests
            var entities = this.GetMovableEntities();
            var moveRequests = new Dictionary<IMovableEntity, MoveRequest>();
            for (int ei = 0; ei < entities.Count; ei++)
            {
                var entity = entities[ei];
                var request = entity.GetMoveRequest(this);
                if (!request.HasValue) { continue; }
                moveRequests.Add(entity, request.Value);
            }
            
            // Reconcile movement

            // TODO: Start with initial data about what positions are blocked/stepable
            var targetCounts = new int[_settings.gridDimensions.x, _settings.gridDimensions.y]; 
            
            foreach (var request in moveRequests.Values)
            {
                targetCounts[request.destination.x, request.destination.z] += 1;
            }
            
            foreach (var pair in moveRequests)
            {
                var entity = pair.Key;
                var request = pair.Value;
                int count = targetCounts[request.destination.x, request.destination.z];

                bool success = count <= 1;

                if (success)
                {
                    // TODO: Store action for movement
                    entity.MoveTo(request.destination);
                }
                else
                {
                    // TODO: Store action for failure
                }
            }
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