using System;

namespace ISTUK.MathLibrary
{
    [Serializable]
    public class MatrixNullReference : NullReferenceException
    {
        public MatrixNullReference(string message)
            : base(message)
        {
        }
        public MatrixNullReference() : base() { }
    }
}