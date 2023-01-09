using System;

namespace AngleConverter.Model
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
