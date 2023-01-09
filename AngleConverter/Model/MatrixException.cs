using System;
using System.ComponentModel;

namespace AngleConverter.Model
{
    [Serializable]
    public sealed class MatrixException : Exception
    {
        public MatrixException([Localizable(false)] string message)
            : base(message)
        {
        }
    }
}
