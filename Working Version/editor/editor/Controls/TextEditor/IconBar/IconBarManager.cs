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
        private readonly ObservableCollection<IBookmark> _bookmarks = new();

        public IconBarManager()
        {
            _bookmarks.CollectionChanged += BookmarksCollectionChanged;
        }

        public event EventHandler RedrawRequested;

        public IList<IBookmark> Bookmarks => _bookmarks;

        public void Redraw() => RedrawRequested?.Invoke(this, EventArgs.Empty);

        private void BookmarksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => Redraw();

        public void AddBookMark(UIElement item)
        {
        }
    }
}