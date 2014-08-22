using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using miRobotEditor.Core;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Classes
{
    /// <summary>
    ///     A bookmark that can be attached to an AvalonEdit TextDocument.
    /// </summary>
    public class BookmarkBase : IBookmark
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] // ReSharper disable InconsistentNaming
        public static readonly IImage defaultBookmarkImage = null;
            // = new ResourceServiceImage("Bookmarks.ToggleMark");

        private IEditor _document;
        private Location _location;

        public BookmarkBase(Location location)
        {
            Location = location;
        }

        public IEditor Document
        {
            get { return _document; }
            set
            {
                if (_document == value) return;
                if (Anchor != null)
                {
                    _location = Anchor.Location;
                    Anchor = null;
                }
                _document = value;
                CreateAnchor();
                OnDocumentChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets the TextAnchor used for this bookmark.
        ///     Is null if the bookmark is not connected to a document.
        /// </summary>
        public ITextAnchor Anchor { get; private set; }

        public Location Location
        {
            get { return Anchor != null ? Anchor.Location : _location; }
            set
            {
                _location = value;
                CreateAnchor();
            }
        }

        public int ColumnNumber
        {
            get { return Anchor != null ? Anchor.Column : _location.Column; }
        }

        /// <summary>
        ///     Gets if the bookmark can be toggled off using the 'set/unset bookmark' command.
        /// </summary>
        public virtual bool CanToggle
        {
            get { return true; }
        }

// ReSharper restore InconsistentNaming

        public static IImage DefaultBookmarkImage
        {
            get { return defaultBookmarkImage; }
        }

        public int LineNumber
        {
            get { return Anchor != null ? Anchor.Line : _location.Line; }
        }

        public virtual int ZOrder
        {
            get { return 0; }
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
            if (e.ChangedButton != MouseButton.Left || !CanToggle) return;
            RemoveMark();
            e.Handled = true;
        }

        public virtual bool CanDragDrop
        {
            get { return false; }
        }

        public virtual void Drop(int lineNumber)
        {
        }

        private void CreateAnchor()
        {
            if (_document != null)
            {
                int lineNumber = Math.Max(1, Math.Min(_location.Line, _document.TotalNumberOfLines));
                int lineLength = _document.GetLine(lineNumber).Length;
                int offset = _document.PositionToOffset(
                    lineNumber,
                    Math.Max(1, Math.Min(_location.Column, lineLength + 1))
                    );
                Anchor = _document.CreateAnchor(offset);
                // after insertion: keep bookmarks after the initial whitespace (see DefaultFormattingStrategy.SmartReplaceLine)
                Anchor.MovementType = AnchorMovementType.AfterInsertion;
                Anchor.Deleted += AnchorDeleted;
            }
            else
            {
                Anchor = null;
            }
        }

        private void AnchorDeleted(object sender, EventArgs e)
        {
            // the anchor just became invalid, so don't try to use it again
            _location = Location.Empty;
            Anchor = null;
            RemoveMark();
        }

        protected virtual void RemoveMark()
        {
            if (_document == null) return;
            var bookmarkMargin = _document.GetService(typeof (IBookmarkMargin)) as IBookmarkMargin;
            if (bookmarkMargin != null)
                bookmarkMargin.Bookmarks.Remove(this);
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
            if (_document == null) return;
            var bookmarkMargin = _document.GetService(typeof (IBookmarkMargin)) as IBookmarkMargin;
            if (bookmarkMargin != null)
                bookmarkMargin.Redraw();
        }
    }
}