using System;
using System.ComponentModel;

namespace ISTUK.MathLibrary
{
    [Serializable]
    public class MatrixException : Exception
    {
        public MatrixException([Localizable(false)] string message):base(message)
        {

        }
    }
}