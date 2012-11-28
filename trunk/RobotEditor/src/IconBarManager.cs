﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace miRobotEditor.RobotEditor
{
    /// <summary>
    /// Stores the entries in the icon bar margin. Multiple icon bar margins
    /// can use the same manager if split view is used.
    /// </summary>
    public class IconBarManager : IBookmarkMargin
    {
        readonly ObservableCollection<IBookmark> bookmarks = new ObservableCollection<IBookmark>();

        public IconBarManager()
        {
            bookmarks.CollectionChanged += bookmarks_CollectionChanged;
        }

        public IList<IBookmark> Bookmarks
        {
            get { return bookmarks; }
        }

        void bookmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Redraw();
        }

        public void Redraw()
        {
            if (RedrawRequested != null)
                RedrawRequested(this, EventArgs.Empty);
        }

        public event EventHandler RedrawRequested;

        public void AddBookMark(System.Windows.UIElement item)
        {

        }

        //	public void UpdateClassMemberBookmarks(ParseInformation parseInfo, TextDocument document)
        //	{
        //		for (int i = bookmarks.Count - 1; i >= 0; i--) {
        //			if (IsClassMemberBookmark(bookmarks[i]))
        //				bookmarks.RemoveAt(i);
        //		}
        //		if (parseInfo == null)
        //			return;
        //		foreach (IClass c in parseInfo.CompilationUnit.Classes) {
        //			AddClassMemberBookmarks(c, document);
        //		}
        //	}

        //	void AddClassMemberBookmarks(IClass c, TextDocument document)
        //		{
        //		throw new NotImplementedException();
        //		if (c.IsSynthetic) return;
        //		if (!c.Region.IsEmpty) {
        //			bookmarks.Add(new ClassBookmark(c, document));
        //		}
        //		foreach (IClass innerClass in c.InnerClasses) {
        //			AddClassMemberBookmarks(innerClass, document);
        //		}
        //		foreach (IMethod m in c.Methods) {
        //			if (m.Region.IsEmpty || m.IsSynthetic) continue;
        //			bookmarks.Add(new ClassMemberBookmark(m, document));
        //		}
        //		foreach (IProperty p in c.Properties) {
        //			if (p.Region.IsEmpty || p.IsSynthetic) continue;
        //			bookmarks.Add(new ClassMemberBookmark(p, document));
        //		}
        //		foreach (IField f in c.Fields) {
        //			if (f.Region.IsEmpty || f.IsSynthetic) continue;
        //			bookmarks.Add(new ClassMemberBookmark(f, document));
        //		}
        //		foreach (IEvent e in c.Events) {
        //			if (e.Region.IsEmpty || e.IsSynthetic) continue;
        //			bookmarks.Add(new ClassMemberBookmark(e, document));
        //		}
        //		}

        static bool IsClassMemberBookmark(IBookmark b)
        {
            throw new NotImplementedException();
            //return b is ClassMemberBookmark || b is ClassBookmark;
        }
    }
}