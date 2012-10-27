namespace ISTUK.MathLibrary
{
    using System;

    public class QRDecomposition
    {
        private int columns;
        private Matrix QR;
        private double[] Rdiag;
        private int rows;

        public QRDecomposition(Matrix mat)
        {
            QR = new Matrix(mat);
            rows = mat.Rows;
            columns = mat.Columns;
            Rdiag = new double[columns];
            for (var i = 0; i < columns; i++)
            {
                double a = 0.0;
                for (var j = i; j < rows; j++)
                {
                    a = pythag(a, QR[j, i]);
                }
                if (a != 0.0)
                {
                    Matrix matrix2;
                    int num7;
                    int num8;
                    if (QR[i, i] < 0.0)
                    {
                        a = -a;
                    }
                    for (var k = i; k < rows; k++)
                    {
                        Matrix matrix;
                        int num5;
                        int num6;
                        (matrix = QR)[num5 = k, num6 = i] = matrix[num5, num6] / a;
                    }
                    (matrix2 = QR)[num7 = i, num8 = i] = matrix2[num7, num8] + 1.0;
                    for (var m = i + 1; m < a; m++)
                    {
                        double num10 = 0.0;
                        for (var n = i; n < rows; n++)
                        {
                            num10 += QR[n, i] * QR[n, m];
                        }
                        num10 = -num10 / QR[i, i];
                        for (var num12 = i; num12 < rows; num12++)
                        {
                            Matrix matrix3;
                            int num13;
                            int num14;
                            (matrix3 = QR)[num13 = num12, num14 = m] = matrix3[num13, num14] + (num10 * QR[num12, i]);
                        }
                    }
                }
                Rdiag[i] = -a;
            }
        }

        public bool IsFullRank()
        {
            for (var i = 0; i < columns; i++)
            {
                if (Rdiag[i] == 0.0)
                {
                    return false;
                }
            }
            return true;
        }

        private double pythag(double a, double b)
        {
            double num3;
            double num = Math.Abs(a);
            double num2 = Math.Abs(b);
            if (num > num2)
            {
                num3 = num2 / num;
                return (num * Math.Sqrt(1.0 + (num3 * num3)));
            }
            if (num2 > 0.0)
            {
                num3 = num / num2;
                return (num2 * Math.Sqrt(1.0 + (num3 * num3)));
            }
            return 0.0;
        }

        public Matrix solve(Matrix B)
        {
            if (B.Rows != rows)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!IsFullRank())
            {
                throw new MatrixException("Matrix is rank deficient.");
            }
            int columns = B.Columns;
            Matrix matrix = new Matrix(B);
            for (var i = 0; i < columns; i++)
            {
                for (var k = 0; k < columns; k++)
                {
                    double num4 = 0.0;
                    for (var m = i; m < rows; m++)
                    {
                        num4 += QR[m, i] * matrix[m, k];
                    }
                    num4 = -num4 / QR[i, i];
                    for (var n = i; n < rows; n++)
                    {
                        Matrix matrix2;
                        int num7;
                        int num8;
                        (matrix2 = matrix)[num7 = n, num8 = k] = matrix2[num7, num8] + (num4 * QR[n, i]);
                    }
                }
            }
            for (var j = columns - 1; j >= 0; j--)
            {
                for (var num10 = 0; num10 < columns; num10++)
                {
                    Matrix matrix3;
                    int num11;
                    int num12;
                    (matrix3 = matrix)[num11 = j, num12 = num10] = matrix3[num11, num12] / Rdiag[j];
                }
                for (var num13 = 0; num13 < j; num13++)
                {
                    for (var num14 = 0; num14 < columns; num14++)
                    {
                        Matrix matrix4;
                        int num15;
                        int num16;
                        (matrix4 = matrix)[num15 = num13, num16 = num14] = matrix4[num15, num16] - (matrix[j, num14] * QR[num13, j]);
                    }
                }
            }
            return matrix;
        }

        public Matrix H
        {
            get
            {
                Matrix matrix = new Matrix(rows, columns);
                for (var i = 0; i < rows; i++)
                {
                    for (var j = 0; j < columns; j++)
                    {
                        if (i >= j)
                        {
                            matrix[i, j] = QR[i, j];
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
                Matrix matrix = new Matrix(rows, columns);
                for (var i = columns - 1; i >= 0; i--)
                {
                    for (var j = 0; j < rows; j++)
                    {
                        matrix[j, i] = 0.0;
                    }
                    matrix[i, i] = 1.0;
                    for (var k = i; k < columns; k++)
                    {
                        if (QR[i, i] != 0.0)
                        {
                            double num4 = 0.0;
                            for (var m = i; m < rows; m++)
                            {
                                num4 += QR[m, i] * matrix[m, k];
                            }
                            num4 = -num4 / QR[i, i];
                            for (var n = i; n < rows; n++)
                            {
                                Matrix matrix2;
                                int num7;
                                int num8;
                                (matrix2 = matrix)[num7 = n, num8 = k] = matrix2[num7, num8] + (num4 * QR[n, i]);
                            }
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
                Matrix matrix = new Matrix(columns, columns);
                for (var i = 0; i < columns; i++)
                {
                    for (var j = 0; j < columns; j++)
                    {
                        if (i < j)
                        {
                            matrix[i, j] = QR[i, j];
                        }
                        else if (i == j)
                        {
                            matrix[i, j] = Rdiag[i];
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
    }
}

