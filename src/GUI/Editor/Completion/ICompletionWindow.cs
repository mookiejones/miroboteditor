﻿using System;

namespace miRobotEditor.GUI.Editor.Completion
{
    public interface ICompletionWindow
    {
        double Width { get; set; }
        double Height { get; set; }
        bool CloseAutomatically { get; set; }
        int StartOffset { get; set; }
        int EndOffset { get; set; }
        event EventHandler Closed;
        void Close();
    }
}
