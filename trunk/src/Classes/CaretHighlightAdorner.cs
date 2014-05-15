using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ICSharpCode.AvalonEdit.Editing;

namespace miRobotEditor.Classes
{
    /// <summary>
    /// Animated rectangle around the caret.
    /// </summary>
    sealed class CaretHighlightAdorner : Adorner
    {
        readonly Pen _pen;
        readonly RectangleGeometry _geometry;

        public CaretHighlightAdorner(TextArea textArea)
            : base(textArea.TextView)
        {
            var min = textArea.Caret.CalculateCaretRectangle();
            min.Offset(-textArea.TextView.ScrollOffset);

            var max = min;
            var size = Math.Max(min.Width, min.Height) * 0.25;
            max.Inflate(size, size);

            _pen = new Pen(TextBlock.GetForeground(textArea.TextView).Clone(), 1);

            _geometry = new RectangleGeometry(min, 2, 2);
            _geometry.BeginAnimation(RectangleGeometry.RectProperty, new RectAnimation(min, max, new Duration(TimeSpan.FromMilliseconds(300))) { AutoReverse = true });
            _pen.Brush.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(200))) { BeginTime = TimeSpan.FromMilliseconds(450) });
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(null, _pen, _geometry);
        }
    }
}
