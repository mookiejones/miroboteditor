using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using miRobotEditor.Classes;
using System.Windows.Input;
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
