namespace miRobotEditor.Interfaces
{
    public interface ISnippetCompletionItem : ICompletionItem
    {
        string Keyword { get; }
    }
}