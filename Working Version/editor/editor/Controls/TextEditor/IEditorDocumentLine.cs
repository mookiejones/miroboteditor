using ICSharpCode.AvalonEdit.Document;

namespace miRobotEditor.Interfaces
{
    public interface IEditorDocumentLine : IDocumentLine
    {
        string Text { get; }
    }
}