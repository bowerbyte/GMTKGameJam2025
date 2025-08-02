using System.Collections.Generic;
using Project.Entities;
using Project.Enums;
using Project.Level;
using Project.Level.Settings;
using UnityEngine;

namespace Project.Core
{
    public class GameManager : MonoBehaviour
    {
        // Static References
        
        public static GameManager Instance;

        // Object References
        
        // inspector
        
        [Header("Settings")]
        [SerializeField] private GameplaySettings _gameplaySettings;
        [SerializeField] private LevelManagerSettings _levelManagerSettings;

        // private
        
        private LevelManager _levelManager;

        private float _stepTimer;
        
        private void Awake()
        {
            GameManager.Instance = this;

            Initialize();
        }
        
        private void Initialize()
        {
            _levelManager = LevelManager.Create(_levelManagerSettings);
            
            _levelManager.transform.SetParent(this.transform);

            DebugAddNRandomEntities(10, EntityType.HarvestBot);
            DebugAddNRandomEntities(16, EntityType.Tree);
            DebugAddNRandomEntities(10, EntityType.Stockpile);
        }

        private void DebugAddNRandomEntities(int n, EntityType type)
        {
            var randomLocations = new HashSet<TileLocation>();
            while (randomLocations.Count < n)
            {
                var location = new TileLocation(UnityEngine.Random.Range(0, _levelManager.GridSize[0]-1), UnityEngine.Random.Range(0, _levelManager.GridSize[1]-1));
                if (!_levelManager.TryGetEntityAt(location, out LevelEntity _) &&
                    TileTypes.HasFlag(_levelManager.GetTileType(location), TileFlags.Walkable))
                {
                    randomLocations.Add(location);   
                }
            }

            foreach (var location in randomLocations)
            {
                _levelManager.AddEntityOfTypeAt(type, location);
            }
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     _levelManager.RunStep();
            // }

            _stepTimer += Time.deltaTime;
            if (_stepTimer >= _gameplaySettings.stepDurationInSeconds)
            {
                _levelManager.RunStep();
                _stepTimer = 0.0f;
            }
        }


    }
}