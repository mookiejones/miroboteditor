using System;
using System.ComponentModel;

namespace miRobotEditor.GUI.AngleConverter
{
    [Localizable(false)]
// ReSharper disable once UnusedMember.Global
// ReSharper disable once InconsistentNaming
    public class QRDecomposition
    {
        private const double EPSILON = 0.0001;
        private readonly int _columns;
        private readonly Matrix _qr;
        private readonly double[] _rdiag;
        private readonly int _rows;

        public QRDecomposition(Matrix mat)
        {
            _qr = new Matrix(mat);
            _rows = mat.Rows;
            _columns = mat.Columns;
            _rdiag = new double[_columns];
            for (int i = 0; i < _columns; i++)
            {
                double a = 0.0;
                for (int j = i; j < _rows; j++)
                {
                    a = Pythag(a, _qr[j, i]);
                }
                if (Math.Abs(a - 0.0) > EPSILON)
                {
                    Matrix matrix2;
                    int num7;
                    int num8;
                    if (_qr[i, i] < 0.0)
                    {
                        a = -a;
                    }
                    for (int k = i; k < _rows; k++)
                    {
                        Matrix matrix;
                        int num5;
                        int num6;
                        (matrix = _qr)[num5 = k, num6 = i] = matrix[num5, num6]/a;
                    }
                    (matrix2 = _qr)[num7 = i, num8 = i] = matrix2[num7, num8] + 1.0;
                    for (int m = i + 1; m < a; m++)
                    {
                        double num10 = 0.0;
                        for (int n = i; n < _rows; n++)
                        {
                            num10 += _qr[n, i]*_qr[n, m];
                        }
                        num10 = -num10/_qr[i, i];
                        for (int num12 = i; num12 < _rows; num12++)
                        {
                            Matrix matrix3;
                            int num13;
                            int num14;
                            (matrix3 = _qr)[num13 = num12, num14 = m] = matrix3[num13, num14] + (num10*_qr[num12, i]);
                        }
                    }
                }
                _rdiag[i] = -a;
            }
        }

        public Matrix H
        {
            get
            {
                var matrix = new Matrix(_rows, _columns);
                for (int i = 0; i < _rows; i++)
                {
                    for (int j = 0; j < _columns; j++)
                    {
                        if (i >= j)
                        {
                            matrix[i, j] = _qr[i, j];
                        }
                        else
                        {
                            matrix[i, j] = 0.0;
                        }
                    }
                }
                return matrix;
            }
        }

        public Matrix Q
        {
            get
            {
                var matrix = new Matrix(_rows, _columns);
                for (int i = _columns - 1; i >= 0; i--)
                {
                    for (int j = 0; j < _rows; j++)
                    {
                        matrix[j, i] = 0.0;
                    }
                    matrix[i, i] = 1.0;
                    for (int k = i; k < _columns; k++)
                    {
                        if (!(Math.Abs(_qr[i, i] - 0.0) > EPSILON)) continue;
                        double num4 = 0.0;
                        for (int m = i; m < _rows; m++)
                        {
                            num4 += _qr[m, i]*matrix[m, k];
                        }
                        num4 = -num4/_qr[i, i];
                        for (int n = i; n < _rows; n++)
                        {
                            Matrix matrix2;
                            int num7;
                            int num8;
                            (matrix2 = matrix)[num7 = n, num8 = k] = matrix2[num7, num8] + (num4*_qr[n, i]);
                        }
                    }
                }
                return matrix;
            }
        }

        public Matrix R
        {
            get
            {
                var matrix = new Matrix(_columns, _columns);
                for (int i = 0; i < _columns; i++)
                {
                    for (int j = 0; j < _columns; j++)
                    {
                        if (i < j)
                        {
                            matrix[i, j] = _qr[i, j];
                        }
                        else if (i == j)
                        {
                            matrix[i, j] = _rdiag[i];
                        }
                        else
                        {
                            matrix[i, j] = 0.0;
                        }
                    }
                }
                return matrix;
            }
        }

        public bool IsFullRank()
        {
            for (int i = 0; i < _columns; i++)
            {
                if (Math.Abs(_rdiag[i] - 0.0) < EPSILON)
                {
                    return false;
                }
            }
            return true;
        }

        private static double Pythag(double a, double b)
        {
            double num3;
            double num = Math.Abs(a);
            double num2 = Math.Abs(b);
            if (num > num2)
            {
                num3 = num2/num;
                return (num*Math.Sqrt(1.0 + (num3*num3)));
            }
            if (num2 > 0.0)
            {
                num3 = num/num2;
                return (num2*Math.Sqrt(1.0 + (num3*num3)));
            }
            return 0.0;
        }

        public Matrix Solve(Matrix b)
        {
            if (b.Rows != _rows)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!IsFullRank())
            {
                throw new MatrixException("Matrix is rank deficient.");
            }
            int columns = b.Columns;
            var matrix = new Matrix(b);
            for (int i = 0; i < columns; i++)
            {
                for (int k = 0; k < columns; k++)
                {
                    double num4 = 0.0;
                    for (int m = i; m < _rows; m++)
                    {
                        num4 += _qr[m, i]*matrix[m, k];
                    }
                    num4 = -num4/_qr[i, i];
                    for (int n = i; n < _rows; n++)
                    {
                        Matrix matrix2;
                        int num7;
                        int num8;
                        (matrix2 = matrix)[num7 = n, num8 = k] = matrix2[num7, num8] + (num4*_qr[n, i]);
                    }
                }
            }
            for (int j = columns - 1; j >= 0; j--)
            {
                for (int num10 = 0; num10 < columns; num10++)
                {
                    Matrix matrix3;
                    int num11;
                    int num12;
                    (matrix3 = matrix)[num11 = j, num12 = num10] = matrix3[num11, num12]/_rdiag[j];
                }
                for (int num13 = 0; num13 < j; num13++)
                {
                    for (int num14 = 0; num14 < columns; num14++)
                    {
                        Matrix matrix4;
                        int num15;
                        int num16;
                        (matrix4 = matrix)[num15 = num13, num16 = num14] = matrix4[num15, num16] -
                                                                           (matrix[j, num14]*_qr[num13, j]);
                    }
                }
            }
            return matrix;
        }
    }
}