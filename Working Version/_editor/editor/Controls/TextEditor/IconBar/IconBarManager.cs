using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Classes
{
    public sealed class IconBarManager : IBookmarkMargin
    {
        private readonly ObservableCollection<IBookmark> _bookmarks = new ObservableCollection<IBookmark>();

        public IconBarManager()
        {
            _bookmarks.CollectionChanged += BookmarksCollectionChanged;
        }

        public event EventHandler RedrawRequested;

        public IList<IBookmark> Bookmarks
        {
            get { return _bookmarks; }
        }

        public void Redraw()
        {
            if (RedrawRequested != null)
            {
                RedrawRequested(this, EventArgs.Empty);
            }
        }

        private void BookmarksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Redraw();
        }

        public void AddBookMark(UIElement item)
        {
        }
    }
}