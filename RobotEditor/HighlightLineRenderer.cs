using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace miRobotEditor.Controls.RobotEditor
{
    /// <summary>
    /// Used for Highlighting Currently selected line in editor
    /// </summary>
    public class HighlightLineRenderer:IBackgroundRenderer 
    {
        /// <summary>
        /// Line To be highlighted
        /// </summary>
        private readonly DocumentLine _line;
        public HighlightLineRenderer(DocumentLine line)
        {
            _line = line;
           
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();

            if (_line.IsDeleted) return;
            var segment = new TextSegment { StartOffset = _line.Offset, EndOffset = _line.EndOffset };
            foreach (var r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
            {
                // Im not using the red brush, just have it here for reference
                drawingContext.DrawRoundedRectangle(Brushes.Yellow, new Pen(Brushes.Red, 0),  new Rect(r.Location,new Size(textView.ActualWidth, r.Height)), 3, 3);
            }
        }

        public KnownLayer Layer { get; private set; }
    }
}
