using System;
using Unity.Mathematics;

namespace Project.Level
{
    [Serializable]
    public struct TileLocation : IEquatable<TileLocation>
    {
        // Properties
        public int x => _value.x;
        public int z => _value.y;
        
        private int2 _value;

        // Constructors
        public TileLocation(int x, int z)
        {
            _value = new int2(x, z);
        }

        public TileLocation(int2 value)
        {
            _value = value;
        }

        // Equality
        public override bool Equals(object obj)
        {
            return obj is TileLocation other && Equals(other);
        }

        public bool Equals(TileLocation other)
        {
            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            return (int)math.hash(_value);
        }

        public static bool operator ==(TileLocation a, TileLocation b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TileLocation a, TileLocation b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return $"({_value.x}, {_value.y})";
        }

        // Conversion operators
        public static implicit operator int2(TileLocation tp) => tp._value;
        public static implicit operator TileLocation(int2 v) => new TileLocation(v);
        
        // Arithmetic operators
        public static TileLocation operator +(TileLocation a, TileLocation b)
        {
            return new TileLocation(a._value + b._value);
        }

        public static TileLocation operator -(TileLocation a, TileLocation b)
        {
            return new TileLocation(a._value - b._value);
        }

        public static TileLocation operator +(TileLocation a, int2 b)
        {
            return new TileLocation(a._value + b);
        }

        public static TileLocation operator -(TileLocation a, int2 b)
        {
            return new TileLocation(a._value - b);
        }

        public static TileLocation operator +(int2 a, TileLocation b)
        {
            return new TileLocation(a + b._value);
        }

        public static TileLocation operator -(int2 a, TileLocation b)
        {
            return new TileLocation(a - b._value);
        }
    }
}