using System;
using Project.Level.Data;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Level.Settings
{
    
    [CreateAssetMenu(fileName = "LevelManagerSettings (New)", menuName = "Settings/Level Manager", order = 1)]
    public class LevelManagerSettings : ScriptableObject
    {
        public int2 gridDimensions = new int2(16);
        

        public LevelData GetInitialLevelData()
        {
            return LevelData.GetEmptyLevelData(gridDimensions);
        }
    }
    
}