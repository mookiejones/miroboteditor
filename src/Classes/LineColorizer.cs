using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace miRobotEditor.Classes
{
    public class LineColorizer : DocumentColorizingTransformer
    {
        public string TextToSelect { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (line.Length == 0)
                return;

            if (line.Offset < StartOffset || line.Offset > EndOffset)
                return;

            var start = line.Offset > StartOffset ? line.Offset : StartOffset;
            var end = EndOffset > line.EndOffset ? line.EndOffset : EndOffset;

            ChangeLinePart(start, end, element => element.TextRunProperties.SetBackgroundBrush(Brushes.Red));

        }
    }
} 

