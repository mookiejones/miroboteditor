using miRobotEditor.Languages;

namespace miRobotEditor.GUI.Editor
{
    /// <summary>
    ///     Completion item that supports complex content and description.
    /// </summary>

// ReSharper disable UnusedMember.Global
    public interface IFancyCompletionItem : ICompletionItem
    {
        object Content { get; }
        new object Description { get; }
    }

    // ReSharper restore UnusedMember.Global
}