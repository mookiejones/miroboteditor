using miRobotEditor.Abstract;
using miRobotEditor.Controls.TextEditor.Bookmarks;

namespace miRobotEditor.Interfaces
{
    public interface ICompletionItem
    {
        string Text { get; }
        string Description { get; }
        IImage Image { get; }
        double Priority { get; }

        void Complete(CompletionContext context);
    }
}