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



        }

    }
}