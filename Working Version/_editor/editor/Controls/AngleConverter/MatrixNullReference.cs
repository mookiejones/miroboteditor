using System;

namespace miRobotEditor.Classes
{
    [Serializable]
    public sealed class MatrixNullReference : NullReferenceException
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