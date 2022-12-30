using ICSharpCode.AvalonEdit.Document;
using miRobotEditor.Controls.TextEditor.Brackets;

namespace miRobotEditor.Interfaces
{
    public interface IBracketSearcher
    {
        BracketSearchResult SearchBracket(TextDocument document, int offset);
    }
}