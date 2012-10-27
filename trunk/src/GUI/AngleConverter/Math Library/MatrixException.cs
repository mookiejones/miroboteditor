using System;

namespace ISTUK.MathLibrary
{
    [Serializable]
    public class MatrixException : Exception
    {
        public MatrixException(string message):base(message)
        {

        }
    }
}