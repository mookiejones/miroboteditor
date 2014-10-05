namespace miRobotEditor.Interfaces
{
    public interface IFancyCompletionItem : ICompletionItem
    {
        object Content { get; }
    }
}