﻿
using System.ComponentModel;
using miRobotEditor.Controls;
using ICSharpCode.AvalonEdit.Folding;

namespace miRobotEditor.Robot
{
    public class RobotFold : NewFolding
    {
        public RobotFold()
        {
        }

        public RobotFold(int start,int end, string message):base(start,end)
        {
            Message = message;
           
        }

        public RobotFold(int start, int end, string text, string message)
            : base(start, end)
        {
            Message = message;
            ToolTip = new CustomToolTip { Title = text, Message = text };
            Text = Text;
        }

        public RobotFold(string message)
        {
            Message = message;
            ToolTip = new CustomToolTip();
        }

        public string Text { get; private set; }
        public string Message { get; private set; }
        public CustomToolTip ToolTip { get; private set; }
       
    }


}
