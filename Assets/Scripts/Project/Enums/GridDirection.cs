using System;
using Unity.Mathematics;

namespace Project.Enums
{
    public enum GridDirection
    {
        Left, Right, Backward, Forward
    }

    public static class GridDirections
    {
        public static int2 GridDirectionToOffset(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.Left:
                    return new int2(-1, 0);
                case GridDirection.Right:
                    return new int2(1, 0);
                case GridDirection.Backward:
                    return new int2(0, -1);
                case GridDirection.Forward:
                    return new int2(0, 1);
                default:
                    throw new NotImplementedException($"Direction {direction} is not implemented");
            }
        }
    }
}