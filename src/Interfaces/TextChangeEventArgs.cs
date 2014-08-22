using System;
using miRobotEditor.Annotations;

namespace miRobotEditor.Interfaces
{
    /// <summary>
    ///     Describes a change of the document text.
    ///     This class is thread-safe.
    /// </summary>
    public abstract class TextChangeEventArgs : EventArgs
    {
        /// <summary>
        ///     Creates a new TextChangeEventArgs object.
        /// </summary>
        protected TextChangeEventArgs(int offset, string removedText, string insertedText)
        {
            Offset = offset;
            RemovedText = removedText ?? string.Empty;
            InsertedText = insertedText ?? string.Empty;
        }

        /// <summary>
        ///     The offset at which the change occurs.
        /// </summary>
        private int Offset { [UsedImplicitly] get; set; }

        /// <summary>
        ///     The text that was inserted.
        /// </summary>
        private string RemovedText { [UsedImplicitly] get; set; }

        /// <summary>
        ///     The text that was inserted.
        /// </summary>
        private string InsertedText { [UsedImplicitly] get; set; }
    }
}