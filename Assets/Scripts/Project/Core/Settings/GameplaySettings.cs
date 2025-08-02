
using Unity.Mathematics;
using UnityEngine;

namespace Project.Level.Settings
{
    
    [CreateAssetMenu(fileName = "Gameplay Settings (New)", menuName = "Settings/Gameplay", order = 0)]
    public class GameplaySettings : ScriptableObject
    {
        [Range(0.1f, 10f)]
        public float stepDurationInSeconds = 1f;

        
    }
    
}