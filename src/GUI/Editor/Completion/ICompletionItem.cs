using System.Linq;
using System.Text;
using System.Threading.Tasks;
using miRobotEditor.Interfaces;

namespace miRobotEditor.GUI.Editor.Completion
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
