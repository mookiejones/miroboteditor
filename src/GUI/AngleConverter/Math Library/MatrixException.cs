using System;
using System.ComponentModel;

namespace miRobotEditor.GUI.AngleConverter
{
    [Serializable]
    public class MatrixException : Exception
    {
        public MatrixException([Localizable(false)] string message):base(message)
        {

        }
    }
}