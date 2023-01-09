﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using miRobotEditor.Controls.TextEditor;

namespace miRobotEditor.Classes
{
    [DebuggerStepThrough]
    public sealed class BackgroundRenderer : IBackgroundRenderer
    {
        private readonly DocumentLine _line;

        public BackgroundRenderer(DocumentLine line)
        {
            _line = line;
        }

        public KnownLayer Layer
        {
            get; // ReSharper disable once UnusedAutoPropertyAccessor.Local
            private set;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();
            if (_line.IsDeleted)
            {
                return;
            }

            TextSegment segment = new TextSegment
            {
                StartOffset = _line.Offset,
                EndOffset = _line.EndOffset
            };
            // ReSharper disable once RedundantArgumentDefaultValue
            foreach (Rect current in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment, false))
            {
                drawingContext.DrawRoundedRectangle(new SolidColorBrush(EditorOptions.Instance.HighlightedLineColor),
                    new Pen(Brushes.Red, 0.0),
                    new Rect(current.Location, new Size(textView.ActualWidth, current.Height)), 3.0, 3.0);
            }
        }
    }
}