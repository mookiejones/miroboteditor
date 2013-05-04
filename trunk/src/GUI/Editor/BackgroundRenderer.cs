﻿// Background Renderer used to Color the Current line
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;
namespace miRobotEditor.GUI
{
    /// <summary>
    /// Used for Highlighting Background
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public sealed class BackgroundRenderer : IBackgroundRenderer
    {

        private readonly DocumentLine _line;

        public BackgroundRenderer(DocumentLine line)
        {
            _line = line;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();

            if (!_line.IsDeleted)
            {
                var segment = new TextSegment { StartOffset = _line.Offset, EndOffset = _line.EndOffset };
                foreach (var r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
                {
                    drawingContext.DrawRoundedRectangle(TextEditorOptions.Instance.HighlightedLineColor, new Pen(Brushes.Red, 0),
                                                        new Rect(r.Location, new Size(textView.ActualWidth, r.Height)), 3, 3);
                }
            }
        }

        public KnownLayer Layer { get; private set; }
    }
}
