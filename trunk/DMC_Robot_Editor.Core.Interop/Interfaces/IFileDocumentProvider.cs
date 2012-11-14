﻿namespace miRobotEditor.Core
{
    /// <summary>
    /// Interface for view contents that provide a text editor document
    /// for one or more <see cref="OpenedFile"/>s.
    /// </summary>
    public interface IFileDocumentProvider
    {
        /// <summary>
        /// Gets the edited document for the specified file.
        /// </summary>
        /// <param name="file">The <see cref="OpenedFile"/> to get the document for.</param>
        /// <returns>The edited document for the specified file, or <c>null</c> if this view does not provide a document for the specified file.</returns>
        IDocument GetDocumentForFile(OpenedFile file);
    }
}