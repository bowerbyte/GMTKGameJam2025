using System;

namespace Project.Enums
{
    public enum EntityType : uint
    {
        Empty = 0,
        
        // Bots
        HarvestBot = 1 | EntityFlags.Placeable | EntityFlags.Obstructing | EntityFlags.Harvestable,
        
        // Crops
        Tree = 11 | EntityFlags.Obstructing | EntityFlags.Harvestable,
        
        // Structures
        Stockpile = 21 | EntityFlags.Obstructing | EntityFlags.Depositable,
        
    }
    
    [Flags]
    public enum EntityFlags : uint
    {
        None        = 0,
        Placeable   = 1 << 16,
        Obstructing = 1 << 17,
        Harvestable = 1 << 18,
        Depositable = 1 << 19,
    }
    
    public static class EntityTypes
    {
        public static bool HasFlag(EntityType entityType, EntityFlags flag)
        {
            return ((uint)entityType & (uint)flag) == (uint)flag;
        }
    }
    
}