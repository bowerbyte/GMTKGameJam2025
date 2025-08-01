using System;

namespace Project.Enums
{
    public enum EntityType : uint
    {
        Empty = 0,
        
        // Bots
        HarvestBot = 1 | EntityFlags.Placeable | EntityFlags.Obstructing,
        
        // Crops
        Tree = 11 | EntityFlags.Obstructing,
    }
    
    [Flags]
    public enum EntityFlags : uint
    {
        None        = 0,
        Placeable   = 1 << 16,
        Obstructing = 1 << 17,
    }
    
    public static class EntityTypes
    {
        public static bool HasFlag(EntityType entityType, EntityFlags flag)
        {
            return ((uint)entityType & (uint)flag) == (uint)flag;
        }
    }
    
}