using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using miRobotEditor.Classes;
namespace miRobotEditor.ViewModel
{
    public class LocalVariablesViewModel:ToolViewModel
    {
        public const string ToolContentId = "LocalVariablesTool";

        public LocalVariablesViewModel() : base("Local Variables")
        {
            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Bottom;
          //  IconSource = Utilities.GetIcon(Global.IconProperty);
        }

    }
}
