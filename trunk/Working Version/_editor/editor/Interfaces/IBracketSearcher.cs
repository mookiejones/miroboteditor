using ICSharpCode.AvalonEdit.Document;
using miRobotEditor.Classes;

namespace miRobotEditor.Interfaces
{
    public interface IBracketSearcher
    {
        BracketSearchResult SearchBracket(TextDocument document, int offset);
    }
}