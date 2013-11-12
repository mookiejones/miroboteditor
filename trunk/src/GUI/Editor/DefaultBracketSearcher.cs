using ICSharpCode.AvalonEdit.Document;

namespace miRobotEditor.GUI.Editor
{
    public class DefaultBracketSearcher : IBracketSearcher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DefaultBracketSearcher DefaultInstance = new DefaultBracketSearcher();
		
        public BracketSearchResult SearchBracket(TextDocument document, int offset)
        {
            return null;
        }
    }
}