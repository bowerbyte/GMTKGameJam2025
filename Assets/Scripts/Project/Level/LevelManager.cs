using System.Collections.Generic;
using Project.Core.Enums;
using Project.Level.Data;
using Project.Level.Settings;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Level
{
    public class LevelManager : MonoBehaviour
    {
        private LevelManagerSettings _settings;

        private LevelData _data;

        private TileObject[,] _tileObjects;

        private Dictionary<TileType, GameObject> _tilePrefabMap;

        public void Initialize(LevelManagerSettings settings)
        {
            _settings = settings;

            _data = _settings.GetInitialLevelData();

            InitializeTilePrefabMap();

            ResetForLevelData(_data);
        }

        private void ResetForLevelData(LevelData data)
        {
            _tileObjects = new TileObject[data.Dimensions.x, data.Dimensions.y];

            for (int xi = 0; xi < data.Dimensions.x; xi++)
            {
                for (int zi = 0; zi < data.Dimensions.y; zi++)
                {
                    _tileObjects[xi, zi] = GetTileObjectForType(data.tiles[xi, zi]);
                    
                    int2 index = new int2(xi, zi);
                    _tileObjects[xi, zi].transform.position = GetTileBasePosition(index);
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

        private float3 GetTileBasePosition(int2 index)
        {
            var dims = _settings.gridDimensions;
            var maxTile = (float2)dims / 2f - 0.5f;
            var minTile = -maxTile;

            float2 tilePercentage = index / (float2)(dims - 1);
            var tilePos = math.lerp(minTile, maxTile, tilePercentage);
            return new float3(tilePos.x, 0f, tilePos.y);
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