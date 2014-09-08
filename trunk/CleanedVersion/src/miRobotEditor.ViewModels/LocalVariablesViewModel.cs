using miRobotEditor.Core;
using miRobotEditor.Core.Classes;

namespace miRobotEditor.Core
{
    public class LocalVariablesViewModel:ToolViewModel
    {
        public const string ToolContentId = "LocalVariablesTool";

        public LocalVariablesViewModel() 
        {
            ContentId = ToolContentId;

          //  IconSource = Utilities.GetIcon(Global.IconProperty);
        }

    }
}
