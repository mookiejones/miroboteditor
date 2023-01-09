using System;

namespace AngleConverterControl.Model
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
