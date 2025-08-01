using System;
using Unity.Mathematics;

namespace Project.Enums
{
    public enum GridDirection : short
    {
        West, East, South, North
    }

    public static class GridDirections
    {
        public static int2 GridDirectionToOffset(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.West:
                    return new int2(-1, 0);
                case GridDirection.East:
                    return new int2(1, 0);
                case GridDirection.South:
                    return new int2(0, -1);
                case GridDirection.North:
                    return new int2(0, 1);
                default:
                    throw new NotImplementedException($"Direction {direction} is not implemented");
            }
        }

        public static GridDirection FlipDirection(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.West:
                    return GridDirection.East;
                case GridDirection.East:
                    return GridDirection.West;
                case GridDirection.South:
                    return GridDirection.North;
                case GridDirection.North:
                    return GridDirection.South;
                default:
                    throw new NotImplementedException($"Direction {direction} is not implemented");
            }
        }
    }
}