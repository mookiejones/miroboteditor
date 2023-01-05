using System;
using System.ComponentModel;

namespace miRobotEditor.Structs
{
    public struct Location : IComparable<Location>, IEquatable<Location>
    {
        public static readonly Location Empty = new Location(-1, -1);

        public Location(int column, int line)
        {
            X = column;
            Y = line;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Line
        {
            get => Y;
            set => Y = value;
        }

        public int Column
        {
            get => X;
            set => X = value;
        }

        public bool IsEmpty => X <= 0 && Y <= 0;

        public int CompareTo(Location other)
        {
            int result = this == other ? 0 : this < other ? -1 : 1;
            return result;
        }

        public bool Equals(Location other)
        {
            return this == other;
        }

        [Localizable(false)]
        public override string ToString()
        {
            return string.Format("(Line {1}, Col {0})", X, Y);
        }

        public override int GetHashCode()
        {
            return (87 * X.GetHashCode()) ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Location && (Location)obj == this;
        }

        public static bool operator ==(Location a, Location b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Location a, Location b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static bool operator <(Location a, Location b)
        {
            return a.Y < b.Y || (a.Y == b.Y && a.X < b.X);
        }

        public static bool operator >(Location a, Location b)
        {
            return a.Y > b.Y || (a.Y == b.Y && a.X > b.X);
        }

        public static bool operator <=(Location a, Location b)
        {
            return !(a > b);
        }

        public static bool operator >=(Location a, Location b)
        {
            return !(a < b);
        }
    }
}