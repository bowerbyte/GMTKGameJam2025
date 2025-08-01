using System;
using Unity.Mathematics;

namespace Project.Level
{
    [Serializable]
    public struct TilePosition : IEquatable<TilePosition>
    {
        // Properties
        public int x => _value.x;
        public int z => _value.y;
        
        private int2 _value;

        // Constructors
        public TilePosition(int x, int z)
        {
            _value = new int2(x, z);
        }

        public TilePosition(int2 value)
        {
            _value = value;
        }

        // Equality
        public override bool Equals(object obj)
        {
            return obj is TilePosition other && Equals(other);
        }

        public bool Equals(TilePosition other)
        {
            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            return (int)math.hash(_value);
        }

        public static bool operator ==(TilePosition a, TilePosition b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TilePosition a, TilePosition b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return $"({_value.x}, {_value.y})";
        }

        // Conversion operators
        public static implicit operator int2(TilePosition tp) => tp._value;
        public static implicit operator TilePosition(int2 v) => new TilePosition(v);
        
        // Arithmetic operators
        public static TilePosition operator +(TilePosition a, TilePosition b)
        {
            return new TilePosition(a._value + b._value);
        }

        public static TilePosition operator -(TilePosition a, TilePosition b)
        {
            return new TilePosition(a._value - b._value);
        }

        public static TilePosition operator +(TilePosition a, int2 b)
        {
            return new TilePosition(a._value + b);
        }

        public static TilePosition operator -(TilePosition a, int2 b)
        {
            return new TilePosition(a._value - b);
        }

        public static TilePosition operator +(int2 a, TilePosition b)
        {
            return new TilePosition(a + b._value);
        }

        public static TilePosition operator -(int2 a, TilePosition b)
        {
            return new TilePosition(a - b._value);
        }
    }
}