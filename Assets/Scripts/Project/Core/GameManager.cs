using System.Collections.Generic;
using Project.Entities;
using Project.Level;
using Project.Level.Enums;
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
        [SerializeField] private LevelManagerSettings _levelManagerSettings;

        // private
        
        private LevelManager _levelManager;
        
        private void Awake()
        {
            GameManager.Instance = this;

            Initialize();
        }
        
        private void Initialize()
        {
            _levelManager = LevelManager.Create(_levelManagerSettings);
            
            _levelManager.transform.SetParent(this.transform);

            _levelManager.AddEntityOfTypeAt(EntityType.HarvestBot, new TilePosition(1,1));

            // Game Simulation Steps
            // (1) Plan Movement
            // (1a) Reconcile Movement
            // (2) Plan Interactions
            // (2a) Reconcile Interactions
            // (3) Execute Movement + Interactions (if possible)
            // (4) Repeat
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _levelManager.RunStep();
            }
        }


    }
}