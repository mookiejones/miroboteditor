using ICSharpCode.AvalonEdit.Document;
using miRobotEditor.Classes;

namespace miRobotEditor.Controls.TextEditor
{
    public sealed class StringTextBuffer : TextSourceAdapter
    {
        public StringTextBuffer(string text)
            : base(new StringTextSource(text))
        {
        }
    }
}