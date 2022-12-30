using System;
using System.Collections.Generic;

namespace miRobotEditor.Interfaces
{
    public interface IBookmarkMargin
    {
        IList<IBookmark> Bookmarks { get; }

        event EventHandler RedrawRequested;

        void Redraw();
    }
}