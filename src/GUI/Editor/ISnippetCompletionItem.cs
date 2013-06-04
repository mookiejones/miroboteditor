namespace miRobotEditor.Languages
{
    public interface ISnippetCompletionItem : ICompletionItem
    {
        string Keyword { get; }
    }
}