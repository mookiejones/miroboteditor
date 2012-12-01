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
        Location _location;

        IDocument _document;
        ITextAnchor _anchor;

        public IDocument Document
        {
            get
            {
                return _document;
            }
            set
            {
                if (_document != value)
                {
                    if (_anchor != null)
                    {
                        _location = _anchor.Location;
                        _anchor = null;
                    }
                    _document = value;
                    CreateAnchor();
                    OnDocumentChanged(EventArgs.Empty);
                }
            }
        }

        void CreateAnchor()
        {
            if (_document != null)
            {
                var lineNumber = Math.Max(1, Math.Min(_location.Line, _document.TotalNumberOfLines));
                var lineLength = _document.GetLine(lineNumber).Length;
                var offset = _document.PositionToOffset(
                    lineNumber,
                    Math.Max(1, Math.Min(_location.Column, lineLength + 1))
                );
                _anchor = _document.CreateAnchor(offset);
                // after insertion: keep bookmarks after the initial whitespace (see DefaultFormattingStrategy.SmartReplaceLine)
                _anchor.MovementType = AnchorMovementType.AfterInsertion;
                _anchor.Deleted += AnchorDeleted;
            }
            else
            {
                _anchor = null;
            }
        }

        void AnchorDeleted(object sender, EventArgs e)
        {
            // the anchor just became invalid, so don't try to use it again
            _location = Location.Empty;
            _anchor = null;
            RemoveMark();
        }

        protected virtual void RemoveMark()
        {
            if (_document != null)
            {
                var bookmarkMargin = _document.GetService(typeof(IBookmarkMargin)) as IBookmarkMargin;
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
            get { return _anchor; }
        }

        public Location Location
        {
            get
            {
                if (_anchor != null)
                    return _anchor.Location;
                return _location;
            }
            set
            {
                _location = value;
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
            if (_document != null)
            {
                var bookmarkMargin = _document.GetService(typeof(IBookmarkMargin)) as IBookmarkMargin;
                if (bookmarkMargin != null)
                    bookmarkMargin.Redraw();
            }
        }

        public int LineNumber
        {
            get
            {
                if (_anchor != null)
                    return _anchor.Line;
                return _location.Line;
            }
        }

        public int ColumnNumber
        {
            get
            {
                if (_anchor != null)
                    return _anchor.Column;
                return _location.Column;
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

#pragma warning disable 649
// ReSharper disable InconsistentNaming
        public static readonly IImage defaultBookmarkImage;// = new ResourceServiceImage("Bookmarks.ToggleMark");
// ReSharper restore InconsistentNaming
#pragma warning restore 649

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
