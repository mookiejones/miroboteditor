/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/14/2012
 * Time: 11:17 AM
 * 
 */
using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
namespace miRobotEditor.GUI
{
	/// <summary>
	/// A document representing a source code file for refactoring.
	/// Line and column counting starts at 1.
	/// Offset counting starts at 0.
	/// </summary>
	public interface IDocument : ITextBuffer, IServiceProvider
	{
		/// <summary>
		/// Gets/Sets the whole text as string.
		/// </summary>
		new string Text { get; set; } // hides TextBuffer.Text to add the setter
		
		/// <summary>
		/// Gets the total number of lines in the document.
		/// </summary>
		int TotalNumberOfLines { get; }
		
		/// <summary>
		/// Gets the document line with the specified number.
		/// </summary>
		/// <param name="lineNumber">The number of the line to retrieve. The first line has number 1.</param>
		IDocumentLine GetLine(int lineNumber);
		
		/// <summary>
		/// Gets the document line that contains the specified offset.
		/// </summary>
		IDocumentLine GetLineForOffset(int offset);
		
		int PositionToOffset(int line, int column);
		Location OffsetToPosition(int offset);
		
		void Insert(int offset, string text);
		void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType);
		void Remove(int offset, int length);
		void Replace(int offset, int length, string newText);
		
		/// <summary>
		/// Make the document combine the following actions into a single
		/// action for undo purposes.
		/// </summary>
		void StartUndoableAction();
		
		/// <summary>
		/// Ends the undoable action started with <see cref="StartUndoableAction"/>.
		/// </summary>
		void EndUndoableAction();
		
		/// <summary>
		/// Creates an undo group. Dispose the returned value to close the undo group.
		/// </summary>
		/// <returns>An object that closes the undo group when Dispose() is called.</returns>
		IDisposable OpenUndoGroup();
		
		/// <summary>
		/// Creates a new text anchor at the specified position.
		/// </summary>
		ITextAnchor CreateAnchor(int offset);
		
		event EventHandler<TextChangeEventArgs> Changing;
		event EventHandler<TextChangeEventArgs> Changed;
	}
	/// <summary>
	/// A read-only view on a (potentially mutable) text buffer.
	/// The IDocument interfaces derives from this interface.
	/// </summary>
	public interface ITextBuffer
	{
		/// <summary>
		/// Gets a version identifier for this text buffer.
		/// Returns null for unversioned text buffers.
		/// </summary>
		ITextBufferVersion Version { get; }
		
		/// <summary>
		/// Creates an immutable snapshot of this text buffer.
		/// Unlike all other methods in this interface, this method is thread-safe.
		/// </summary>
		ITextBuffer CreateSnapshot();
		
		/// <summary>
		/// Creates an immutable snapshot of a part of this text buffer.
		/// Unlike all other methods in this interface, this method is thread-safe.
		/// </summary>
		ITextBuffer CreateSnapshot(int offset, int length);
		
		/// <summary>
		/// Creates a new TextReader to read from this text buffer.
		/// </summary>
		TextReader CreateReader();
		
		/// <summary>
		/// Creates a new TextReader to read from this text buffer.
		/// </summary>
		TextReader CreateReader(int offset, int length);
		
		/// <summary>
		/// Gets the total text length.
		/// </summary>
		/// <returns>The length of the text, in characters.</returns>
		/// <remarks>This is the same as Text.Length, but is more efficient because
		///  it doesn't require creating a String object.</remarks>
		int TextLength { get; }
		
		/// <summary>
		/// Gets the whole text as string.
		/// </summary>
		string Text { get; }
		
		/// <summary>
		/// Is raised when the Text property changes.
		/// </summary>
		event EventHandler TextChanged;
		
		/// <summary>
		/// Gets a character at the specified position in the document.
		/// </summary>
		/// <paramref name="offset">The index of the character to get.</paramref>
		/// <exception cref="ArgumentOutOfRangeException">Offset is outside the valid range (0 to TextLength-1).</exception>
		/// <returns>The character at the specified position.</returns>
		/// <remarks>This is the same as Text[offset], but is more efficient because
		///  it doesn't require creating a String object.</remarks>
		char GetCharAt(int offset);
		
		/// <summary>
		/// Retrieves the text for a portion of the document.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">offset or length is outside the valid range.</exception>
		/// <remarks>This is the same as Text.Substring, but is more efficient because
		///  it doesn't require creating a String object for the whole document.</remarks>
		string GetText(int offset, int length);
	}
/// <summary>
	/// Represents a version identifier for a text buffer.
	/// </summary>
	/// <remarks>
	/// This is SharpDevelop's equivalent to AvalonEdit ChangeTrackingCheckpoint.
	/// It is used by the ParserService to efficiently detect whether a document has changed and needs reparsing.
	/// It is a separate class from ITextBuffer to allow the GC to collect the text buffer while the version checkpoint
	/// is still in use.
	/// </remarks>
	public interface ITextBufferVersion
	{
		/// <summary>
		/// Gets whether this checkpoint belongs to the same document as the other checkpoint.
		/// </summary>
		bool BelongsToSameDocumentAs(ITextBufferVersion other);
		
		/// <summary>
		/// Compares the age of this checkpoint to the other checkpoint.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this version.</exception>
		/// <returns>-1 if this version is older than <paramref name="other"/>.
		/// 0 if <c>this</c> version instance represents the same version as <paramref name="other"/>.
		/// 1 if this version is newer than <paramref name="other"/>.</returns>
		int CompareAge(ITextBufferVersion other);
		
		/// <summary>
		/// Gets the changes from this checkpoint to the other checkpoint.
		/// If 'other' is older than this checkpoint, reverse changes are calculated.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this checkpoint.</exception>
		IEnumerable<TextChangeEventArgs> GetChangesTo(ITextBufferVersion other);
		
		/// <summary>
		/// Calculates where the offset has moved in the other buffer version.
		/// </summary>
		/// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this checkpoint.</exception>
		int MoveOffsetTo(ITextBufferVersion other, int oldOffset, AnchorMovementType movement);
	}
/// <summary>
	/// A line inside a <see cref="IDocument"/>.
	/// </summary>
	public interface IDocumentLine
	{
		/// <summary>
		/// Gets the starting offset of the line in the document's text.
		/// </summary>
		int Offset { get; }
		
		/// <summary>
		/// Gets the length of this line (=the number of characters on the line).
		/// </summary>
		int Length { get; }
		
		/// <summary>
		/// Gets the ending offset of the line in the document's text (= Offset + Length).
		/// </summary>
		int EndOffset { get; }
		
		/// <summary>
		/// Gets the length of this line, including the line delimiter.
		/// </summary>
		int TotalLength { get; }
		
		/// <summary>
		/// Gets the length of the line terminator.
		/// Returns 1 or 2; or 0 at the end of the document.
		/// </summary>
		int DelimiterLength { get; }
		
		/// <summary>
		/// Gets the number of this line.
		/// The first line has the number 1.
		/// </summary>
		int LineNumber { get; }
		
		/// <summary>
		/// Gets the text on this line.
		/// </summary>
		string Text { get; }
	}
	/// <summary>
	/// A line/column position.
	/// NRefactory lines/columns are counting from one.
	/// </summary>
	public struct Location : IComparable<Location>, IEquatable<Location>
	{
		public static readonly Location Empty = new Location(-1, -1);
		
		public Location(int column, int line)
		{
			_x = column;
			_y = line;
		}
		
		int _x, _y;
		
		public int X {
			get { return _x; }
			set { _x = value; }
		}
		
		public int Y {
			get { return _y; }
			set { _y = value; }
		}
		
		public int Line {
			get { return Y; }
			set { Y = value; }
		}
		
		public int Column {
			get { return X; }
			set { X = value; }
		}
		
		public bool IsEmpty {
			get {
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
			return unchecked (87 * X.GetHashCode() ^ Y.GetHashCode());
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
			return a.X != b._x || a.Y != b._y;
		}
		
		public static bool operator <(Location a, Location b)
		{
		    if (a.Y < b._y)
				return true;
		    return a.Y == b._y && a.X < b._x;
		}

	    public static bool operator >(Location a, Location b)
	    {
	        if (a.Y > b._y)
				return true;
	        return a.Y == b._y && a.X > b._x;
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
		    return this < other ? -1 : 1;
		}
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
		Default = ICSharpCode.AvalonEdit.Document.AnchorMovementType.Default,
		/// <summary>
		/// Behaves like a start marker - when text is inserted at the anchor position, the anchor will stay
		/// before the inserted text.
		/// </summary>
		BeforeInsertion = ICSharpCode.AvalonEdit.Document.AnchorMovementType.BeforeInsertion,
		/// <summary>
		/// Behave like an end marker - when text is insered at the anchor position, the anchor will move
		/// after the inserted text.
		/// </summary>
		AfterInsertion = ICSharpCode.AvalonEdit.Document.AnchorMovementType.AfterInsertion
	}
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
	/// Describes a change of the document text.
	/// This class is thread-safe.
	/// </summary>
	public class TextChangeEventArgs : EventArgs
	{
		/// <summary>
		/// The offset at which the change occurs.
		/// </summary>
		public int Offset { get; private set; }
		
		/// <summary>
		/// The text that was inserted.
		/// </summary>
		public string RemovedText { get; private set; }
		
		/// <summary>
		/// The number of characters removed.
		/// </summary>
		public int RemovalLength {
			get { return RemovedText.Length; }
		}
		
		/// <summary>
		/// The text that was inserted.
		/// </summary>
		public string InsertedText { get; private set; }
		
		/// <summary>
		/// The number of characters inserted.
		/// </summary>
		public int InsertionLength {
			get { return InsertedText.Length; }
		}
		
		/// <summary>
		/// Creates a new TextChangeEventArgs object.
		/// </summary>
		public TextChangeEventArgs(int offset, string removedText, string insertedText)
		{
			Offset = offset;
			RemovedText = removedText ?? string.Empty;
			InsertedText = insertedText ?? string.Empty;
		}
	}
}
