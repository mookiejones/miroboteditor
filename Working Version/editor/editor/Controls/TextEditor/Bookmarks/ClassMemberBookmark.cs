﻿using System.Windows.Input;
using miRobotEditor.Controls.TextEditor.Bookmarks;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Classes
{
    public sealed class ClassMemberBookmark : IBookmark
    {
        public ClassMemberBookmark(int lineNumber, IImage image)
        {
            Image = image;
            LineNumber = lineNumber;
        }

        public int LineNumber { get; private set; }
        public IImage Image { get; private set; }

        public int ZOrder => -10;

        public bool CanDragDrop => false;

        public void MouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                e.Handled = true;
            }
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
        }

        public void Drop(int lineNumber)
        {
        }
    }
}