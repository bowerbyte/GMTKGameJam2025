using System;

namespace Project.Level.Enums
{

    public enum TileType : uint
    {
        Empty = 0,
        
        Grass = 1 | TileFlags.Walkable,
    }
    
    [Flags]
    public enum TileFlags : uint
    {
        None        = 0,
        Walkable    = 1 << 16,
    }

    public static class TileTypes
    {
        public static bool HasFlag(TileType tileType, TileFlags flag)
        {
            return ((uint)tileType & (uint)flag) == (uint)flag;
        }
    }
}