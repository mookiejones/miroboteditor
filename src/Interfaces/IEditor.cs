﻿using System;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Document;

namespace miRobotEditor.Interfaces
{
    /// <summary>
    ///     A document representing a source code file for refactoring.
    ///     Line and column counting starts at 1.
    ///     Offset counting starts at 0.
    /// </summary>
    public interface IEditor : ITextBuffer, IServiceProvider
    {
        /// <summary>
        ///     Gets/Sets the whole text as string.
        /// </summary>
        new string Text { get; set; } // hides TextBuffer.Text to add the setter

        /// <summary>
        ///     Gets the total number of lines in the document.
        /// </summary>
        int TotalNumberOfLines { get; }

        /// <summary>
        ///     Gets the document line with the specified number.
        /// </summary>
        /// <param name="lineNumber">The number of the line to retrieve. The first line has number 1.</param>
        IDocumentLine GetLine(int lineNumber);

        /// <summary>
        ///     Gets the document line that contains the specified offset.
        /// </summary>
        IDocumentLine GetLineForOffset(int offset);

        int PositionToOffset(int line, int column);
        Location OffsetToPosition(int offset);

        [Localizable(false)]
        void Insert(int offset, string text);

        void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType);
        void Remove(int offset, int length);
        void Replace(int offset, int length, string newText);

        /// <summary>
        ///     Make the document combine the following actions into a single
        ///     action for undo purposes.
        /// </summary>
        void StartUndoableAction();

        /// <summary>
        ///     Ends the undoable action started with <see cref="StartUndoableAction" />.
        /// </summary>
        void EndUndoableAction();

        /// <summary>
        ///     Creates an undo group. Dispose the returned value to close the undo group.
        /// </summary>
        /// <returns>An object that closes the undo group when Dispose() is called.</returns>
        IDisposable OpenUndoGroup();

        /// <summary>
        ///     Creates a new text anchor at the specified position.
        /// </summary>
        ITextAnchor CreateAnchor(int offset);

        event EventHandler<TextChangeEventArgs> Changing;
        event EventHandler<TextChangeEventArgs> Changed;
    }
}