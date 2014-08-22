using System.Diagnostics.CodeAnalysis;
using ICSharpCode.AvalonEdit.Document;
using miRobotEditor.GUI.Editor;

namespace miRobotEditor.GUI
{
// ReSharper disable once UnusedMember.Global
    public class DefaultBracketSearcher : IBracketSearcher
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly DefaultBracketSearcher DefaultInstance = new DefaultBracketSearcher();

        public BracketSearchResult SearchBracket(TextDocument document, int offset)
        {
            return null;
        }
    }
}