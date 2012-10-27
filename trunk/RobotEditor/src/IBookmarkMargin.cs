using System;
using System.Collections.Generic;
using miRobotEditor.RobotEditor;

namespace miRobotEditor
{

    /// <summary>
    /// The bookmark margin.
    /// </summary>
    public interface IBookmarkMargin
    {
        /// <summary>
        /// Gets the list of bookmarks.
        /// </summary>
        IList<IBookmark> Bookmarks { get; }

        /// <summary>
        /// Redraws the bookmark margin. Bookmarks need to call this method when the Image changes.
        /// </summary>
        void Redraw();

        event EventHandler RedrawRequested;
    }
}
