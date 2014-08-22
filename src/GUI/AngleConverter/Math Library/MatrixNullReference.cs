using System;

namespace miRobotEditor.GUI.AngleConverter
{
    [Serializable]
    public class MatrixNullReference : NullReferenceException
    {
        public MatrixNullReference(string message)
            : base(message)
        {
        }

        public MatrixNullReference()
        {
        }
    }
}