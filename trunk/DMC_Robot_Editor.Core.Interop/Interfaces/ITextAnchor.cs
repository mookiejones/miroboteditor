using System;
using System.ComponentModel;

namespace miRobotEditor.Core
{
    /// <summary>
    /// Represents an anchored location inside an <see cref="IDocument"/>.
    /// </summary>
    public interface ITextAnchor
    {
        /// <summary>
        /// Gets the text location of this anchor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
        Location Location { get; }

        /// <summary>
        /// Gets the offset of the text anchor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
        int Offset { get; }

        /// <summary>
        /// Controls how the anchor moves.
        /// </summary>
        AnchorMovementType MovementType { get; set; }

        /// <summary>
        /// Specifies whether the anchor survives deletion of the text containing it.
        /// <c>false</c>: The anchor is deleted when the a selection that includes the anchor is deleted.
        /// <c>true</c>: The anchor is not deleted.
        /// </summary>
        bool SurviveDeletion { get; set; }

        /// <summary>
        /// Gets whether the anchor was deleted.
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Occurs after the anchor was deleted.
        /// </summary>
        event EventHandler Deleted;

        /// <summary>
        /// Gets the line number of the anchor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
        int Line { get; }

        /// <summary>
        /// Gets the column number of this anchor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
        int Column { get; }
    }

    /// <summary>
    /// Defines how a text anchor moves.
    /// </summary>
    public enum AnchorMovementType
    {
      
        /// <summary>
        /// When text is inserted at the anchor position, the type of the insertion
        /// determines where the caret moves to. For normal insertions, the anchor will stay
        /// behind the inserted text.
        /// </summary>
        Default,
        /// <summary>
        /// Behaves like a start marker - when text is inserted at the anchor position, the anchor will stay
        /// before the inserted text.
        /// </summary>
        BeforeInsertion,
        /// <summary>
        /// Behave like an end marker - when text is insered at the anchor position, the anchor will move
        /// after the inserted text.
        /// </summary>
        AfterInsertion
    }
    public struct Location : IComparable<Location>, IEquatable<Location>
    {
        public static readonly Location Empty = new Location(-1, -1);

        public Location(int column, int line) : this()
        {

            X = column;
            Y = line;
        }



        public int X { get; set; }

        public int Y { get; set; }

        public int Line
        {
            get { return Y; }
            set {Y = value; }
        }

        public int Column
        {
            get { return X; }
            set { X = value; }
        }

        public bool IsEmpty
        {
            get
            {
                return X <= 0 && Y <= 0;
            }
        }

        [Localizable(false)]
        public override string ToString()
        {
            return string.Format("(Line {1}, Col {0})", X, Y);
        }

        public override int GetHashCode()
        {
            return unchecked(87 * X.GetHashCode() ^ Y.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Location)) return false;
            return (Location)obj == this;
        }

        public bool Equals(Location other)
        {
            return this == other;
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
            if (a.Y < b.Y)
                return true;
            if (a.Y == b.Y)
                return a.X < b.X;
            return false;
        }

        public static bool operator >(Location a, Location b)
        {
            if (a.Y > b.Y)
                return true;
            if (a.Y == b.Y)
                return a.X > b.X;
            return false;
        }

        public static bool operator <=(Location a, Location b)
        {
            return !(a > b);
        }

        public static bool operator >=(Location a, Location b)
        {
            return !(a < b);
        }

        public int CompareTo(Location other)
        {
            if (this == other)
                return 0;
            if (this < other)
                return -1;
            return 1;
        }
    }
}
