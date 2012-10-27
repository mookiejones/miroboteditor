using System;
using System.Windows.Input;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Core
{
    /// <summary>
    /// A bookmark that can be attached to an AvalonEdit TextDocument.
    /// </summary>
    public class BookmarkBase : IBookmark
    {
        Location location;

        IDocument document;
        ITextAnchor anchor;

        public IDocument Document
        {
            get
            {
                return document;
            }
            set
            {
                if (document != value)
                {
                    if (anchor != null)
                    {
                        location = anchor.Location;
                        anchor = null;
                    }
                    document = value;
                    CreateAnchor();
                    OnDocumentChanged(EventArgs.Empty);
                }
            }
        }

        void CreateAnchor()
        {
            if (document != null)
            {
                int lineNumber = Math.Max(1, Math.Min(location.Line, document.TotalNumberOfLines));
                int lineLength = document.GetLine(lineNumber).Length;
                int offset = document.PositionToOffset(
                    lineNumber,
                    Math.Max(1, Math.Min(location.Column, lineLength + 1))
                );
                anchor = document.CreateAnchor(offset);
                // after insertion: keep bookmarks after the initial whitespace (see DefaultFormattingStrategy.SmartReplaceLine)
                anchor.MovementType = AnchorMovementType.AfterInsertion;
                anchor.Deleted += AnchorDeleted;
            }
            else
            {
                anchor = null;
            }
        }

        void AnchorDeleted(object sender, EventArgs e)
        {
            // the anchor just became invalid, so don't try to use it again
            location = Location.Empty;
            anchor = null;
            RemoveMark();
        }

        protected virtual void RemoveMark()
        {
            if (document != null)
            {
                var bookmarkMargin = document.GetService(typeof(IBookmarkMargin)) as IBookmarkMargin;
                if (bookmarkMargin != null)
                    bookmarkMargin.Bookmarks.Remove(this);
            }
        }

        /// <summary>
        /// Gets the TextAnchor used for this bookmark.
        /// Is null if the bookmark is not connected to a document.
        /// </summary>
        public ITextAnchor Anchor
        {
            get { return anchor; }
        }

        public Location Location
        {
            get
            {
                if (anchor != null)
                    return anchor.Location;
                return location;
            }
            set
            {
                location = value;
                CreateAnchor();
            }
        }

        public event EventHandler DocumentChanged;

        protected virtual void OnDocumentChanged(EventArgs e)
        {
            if (DocumentChanged != null)
            {
                DocumentChanged(this, e);
            }
        }

        protected virtual void Redraw()
        {
            if (document != null)
            {
                var bookmarkMargin = document.GetService(typeof(IBookmarkMargin)) as IBookmarkMargin;
                if (bookmarkMargin != null)
                    bookmarkMargin.Redraw();
            }
        }

        public int LineNumber
        {
            get
            {
                if (anchor != null)
                    return anchor.Line;
                return location.Line;
            }
        }

        public int ColumnNumber
        {
            get
            {
                if (anchor != null)
                    return anchor.Column;
                return location.Column;
            }
        }

        public virtual int ZOrder
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets if the bookmark can be toggled off using the 'set/unset bookmark' command.
        /// </summary>
        public virtual bool CanToggle
        {
            get
            {
                return true;
            }
        }

        public BookmarkBase(Location location)
        {
            Location = location;
        }

        public static readonly IImage defaultBookmarkImage;// = new ResourceServiceImage("Bookmarks.ToggleMark");

        public static IImage DefaultBookmarkImage
        {
            get { return defaultBookmarkImage; }
        }

        public virtual IImage Image
        {
            get { return defaultBookmarkImage; }
        }

        public virtual void MouseDown(MouseButtonEventArgs e)
        {
        }

        public virtual void MouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && CanToggle)
            {
                RemoveMark();
                e.Handled = true;
            }
        }

        public virtual bool CanDragDrop
        {
            get { return false; }
        }

        public virtual void Drop(int lineNumber)
        {
        }
    }

}
