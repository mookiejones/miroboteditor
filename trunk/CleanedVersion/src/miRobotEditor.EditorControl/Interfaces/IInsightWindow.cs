using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using miRobotEditor.Interfaces;

namespace miRobotEditor.EditorControl.Interfaces
{
    /// <summary>
    /// Describes a set of insight items (e.g. multiple overloads of a method) to be displayed in the insight window.
    /// </summary>
    public interface IInsightWindow : ICompletionWindow
    {
        /// <summary>
        /// Gets the items to display.
        /// </summary>
        IList<IInsightItem> Items { get; }

        /// <summary>
        /// Gets/Sets the item that is currently selected.
        /// </summary>
        IInsightItem SelectedItem { get; set; }

        /// <summary>
        /// Occurs when the document is changed while the insight window is open.
        /// Use this event to close the insight window or adjust .
        /// </summary>
        /// <remarks>
        /// Unlike directly attaching to <see cref="IEditor.TextChanged"/>, using the event does not require handlers to unsubscribe
        /// when the insight window is closed. This makes it easier to avoid memory leaks.
        /// </remarks>
        event EventHandler<TextChangeEventArgs> DocumentChanged;

        event EventHandler SelectedItemChanged;

        event EventHandler CaretPositionChanged;
    }
}
