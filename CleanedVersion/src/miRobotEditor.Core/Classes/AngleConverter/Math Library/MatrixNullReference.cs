using System;

namespace miRobotEditor.Core.Classes.AngleConverter
{
    [Serializable]
    public class MatrixNullReference : NullReferenceException
    {
        public MatrixNullReference(string message)
            : base(message)
        {
        }
        public MatrixNullReference()
        { }
    }
}