using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class FunctionViewModel : ToolViewModel
    {

        public const string ToolContentId = "FunctionTool";

        public FunctionViewModel() : base("Functions")
        {
            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Right;

        }
    }

}
