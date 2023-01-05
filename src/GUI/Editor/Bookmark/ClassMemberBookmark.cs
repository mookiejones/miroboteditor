using miRobotEditor.Interfaces;
using System.Windows.Input;

namespace miRobotEditor.GUI.Editor.Bookmark
{
    /// <summary>
    ///     Description of ClassMemberBookmark.
    /// </summary>
    public sealed class ClassMemberBookmark : IBookmark
    {
        public ClassMemberBookmark(int lineNumber, IImage image)
        {
            Image = image;
            LineNumber = lineNumber;
        }

        public int LineNumber { get; private set; }

        public IImage Image { get; private set; }

        public int ZOrder => -10;

        public bool CanDragDrop => false;

        public void MouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                //	var f = Services.AnalyticsMonitorService.TrackFeature("ICSharpCode.SharpDevelop.Bookmarks.ClassMemberBookmark.ShowContextMenu");
                //TODO Fix this

                //var ctx = MenuService.ShowContextMenu(e.Source as UIElement, this, ContextMenuPath);
                //		ctx.Closed += delegate { f.EndTracking(); };
                e.Handled = true;
            }
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
        }

        public void Drop(int lineNumber)
        {
        }
    }
}
