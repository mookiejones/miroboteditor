namespace ISTUK.MathLibrary
{
    using System;

    public class SVD
    {
        private Matrix u;
        private SquareMatrix v;
        private Vector w;

        public SVD(Matrix mat)
        {
            double num9,num12,num13;
            Matrix matrix;
            int num17,num18,num3 = 0;

            if (mat.Rows < mat.Columns)
            {
                throw new MatrixException("Matrix must have rows >= columns");
            }
            int rows = mat.Rows;
            int columns = mat.Columns;
           
            Vector vector = new Vector(columns);
            double num4 = 0.0;
            double b = 0.0;
            double num6 = 0.0;
            int num7 = 0;
            u = new Matrix(mat);
            v = new SquareMatrix(columns);
            w = new Vector(columns);
            for (var i = 0; i < columns; i++)
            {
                num3 = i + 1;
                vector[i] = num4 * b;
                b = num9 = num4 = 0.0;
                if (i < rows)
                {
                    for (var n = i; n < rows; n++)
                    {
                        num4 += Math.Abs(u[n, i]);
                    }
                    if (num4 != 0.0)
                    {
                        for (var num11 = i; num11 < rows; num11++)
                        {
                            u[num11, i] /= num4;
                            num9 += u[num11, i] * u[num11, i];
                        }
                        num12 = u[i, i];
                        b = (num12 < 0.0) ? Math.Sqrt(num9) : -Math.Sqrt(num9);
                        num13 = (num12 * b) - num9;
                        u[i, i] = num12 - b;
                        if (i != (columns - 1))
                        {
                            for (var num14 = num3; num14 < columns; num14++)
                            {
                                num9 = 0.0;
                                for (var num15 = i; num15 < rows; num15++)
                                {
                                    num9 += u[num15, i] * u[num15, num14];
                                }
                                num12 = num9 / num13;
                                for (var num16 = i; num16 < rows; num16++)
                                {
                                    (matrix = u)[num17 = num16, num18 = num14] = matrix[num17, num18] + (num12 * u[num16, i]);
                                }
                            }
                        }
                        for (var num19 = i; num19 < rows; num19++)
                        {
                            u[num19, i] *= num4;
                        }
                    }
                }
                w[i] = num4 * b;
                b = num9 = num4 = 0.0;
                if ((i < rows) && (i != (columns - 1)))
                {
                    for (var num20 = num3; num20 < columns; num20++)
                    {
                        num4 += Math.Abs(u[i, num20]);
                    }
                    if (num4 != 0.0)
                    {
                        for (var num21 = num3; num21 < columns; num21++)
                        {
                            u[i, num21] /= num4;
                            num9 += u[i, num21] * u[i, num21];
                        }
                        num12 = u[i, num3];
                        b = (num12 < 0.0) ? Math.Sqrt(num9) : -Math.Sqrt(num9);
                        num13 = (num12 * b) - num9;
                        u[i, num3] = num12 - b;
                        for (var num22 = num3; num22 < columns; num22++)
                        {
                            vector[num22] = u[i, num22] / num13;
                        }
                        if (i != (rows - 1))
                        {
                            for (var num23 = num3; num23 < rows; num23++)
                            {
                                num9 = 0.0;
                                for (var num24 = num3; num24 < columns; num24++)
                                {
                                    num9 += u[num23, num24] * u[i, num24];
                                }
                                for (var num25 = num3; num25 < columns; num25++)
                                {
                                    Matrix matrix2;
                                    int num26;
                                    int num27;
                                    (matrix2 = u)[num26 = num23, num27 = num25] = matrix2[num26, num27] + (num9 * vector[num25]);
                                }
                            }
                        }
                        for (var num28 = num3; num28 < columns; num28++)
                        {
                            u[i, num28] *= num4;
                        }
                    }
                }
                num6 = Math.Max(num6, Math.Abs(w[i]) + Math.Abs(vector[i]));
            }
            for (var j = columns - 1; j >= 0; j--)
            {
                if (j < (columns - 1))
                {
                    if (b != 0.0)
                    {
                        for (var num30 = num3; num30 < columns; num30++)
                        {
                            v[num30, j] = (u[j, num30] / u[j, num3]) / b;
                        }
                        for (var num31 = num3; num31 < columns; num31++)
                        {
                            num9 = 0.0;
                            for (var num32 = num3; num32 < columns; num32++)
                            {
                                num9 += u[j, num32] * v[num32, num31];
                            }
                            for (var num33 = num3; num33 < columns; num33++)
                            {
                                Matrix matrix3;
                                int num34;
                                (matrix3 = v)[num34 = num33, num17 = num31] = matrix3[num34, num17] + (num9 * v[num33, j]);
                            }
                        }
                    }
                    for (var num35 = num3; num35 < columns; num35++)
                    {
                        v[j, num35] = v[num35, j] = 0.0;
                    }
                }
                v[j, j] = 1.0;
                b = vector[j];
                num3 = j;
            }
            for (var k = columns - 1; k >= 0; k--)
            {
                num3 = k + 1;
                b = w[k];
                if (k < (columns - 1))
                {
                    for (var num38 = num3; num38 < columns; num38++)
                    {
                        u[k, num38] = 0.0;
                    }
                }
                if (b != 0.0)
                {
                    b = 1.0 / b;
                    if (k != (columns - 1))
                    {
                        for (var num39 = num3; num39 < columns; num39++)
                        {
                            num9 = 0.0;
                            for (var num40 = num3; num40 < rows; num40++)
                            {
                                num9 += u[num40, k] * u[num40, num39];
                            }
                            num12 = (num9 / u[k, k]) * b;
                            for (var num41 = k; num41 < rows; num41++)
                            {
                                (matrix = u)[num17 = num41, num18 = num39] = matrix[num17, num18] + (num12 * u[num41, k]);
                            }
                        }
                    }
                    for (var num42 = k; num42 < rows; num42++)
                    {
                        u[num42, k] *= b;
                    }
                }
                else
                {
                    for (var num43 = k; num43 < rows; num43++)
                    {
                        u[num43, k] = 0.0;
                    }
                }
                (matrix = u)[num17 = k, num18 = k] = matrix[num17, num18] + 1.0;
            }
            for (var m = columns - 1; m >= 0; m--)
            {
                for (var num45 = 0; num45 < 30; num45++)
                {
                    double num47;
                    double num50;
                    double num51;
                    int num46 = 1;
                    num3 = m;
                    while (num3 >= 0)
                    {
                        num7 = num3 - 1;
                        if ((Math.Abs(vector[num3]) + num6) == num6)
                        {
                            num46 = 0;
                            break;
                        }
                        if ((Math.Abs(w[num7]) + num6) == num6)
                        {
                            break;
                        }
                        num3--;
                    }
                    if (num46 != 0)
                    {
                        num47 = 0.0;
                        num9 = 1.0;
                        for (var num48 = num3; num48 <= m; num48++)
                        {
                            num12 = num9 * vector[num48];
                            if ((Math.Abs(num12) + num6) != num6)
                            {
                                b = w[num48];
                                num13 = pythag(num12, b);
                                w[num48] = (float) num13;
                                num13 = 1.0 / num13;
                                num47 = b * num13;
                                num9 = -num12 * num13;
                                for (var num49 = 0; num49 < rows; num49++)
                                {
                                    num50 = u[num49, num7];
                                    num51 = u[num49, num48];
                                    u[num49, num7] = (num50 * num47) + (num51 * num9);
                                    u[num49, num48] = (num51 * num47) - (num50 * num9);
                                }
                            }
                        }
                    }
                    num51 = w[m];
                    if (num3 == m)
                    {
                        if (num51 < 0.0)
                        {
                            w[m] = -num51;
                            for (var num52 = 0; num52 < columns; num52++)
                            {
                                v[num52, m] = -v[num52, m];
                            }
                        }
                        break;
                    }
                    if (num45 >= 30)
                    {
                        throw new MatrixException("No convergence after 30 iterations");
                    }
                    double num53 = w[num3];
                    num7 = m - 1;
                    num50 = w[num7];
                    b = vector[num7];
                    num13 = vector[m];
                    num12 = (((num50 - num51) * (num50 + num51)) + ((b - num13) * (b + num13))) / ((2.0 * num13) * num50);
                    b = pythag(num12, 1.0);
                    double num54 = (num12 >= 0.0) ? b : -b;
                    num12 = (((num53 - num51) * (num53 + num51)) + (num13 * ((num50 / (num12 + num54)) - num13))) / num53;
                    num47 = num9 = 1.0;
                    for (var num55 = num3; num55 <= num7; num55++)
                    {
                        int num56 = num55 + 1;
                        b = vector[num56];
                        num50 = w[num56];
                        num13 = num9 * b;
                        b = num47 * b;
                        num51 = pythag(num12, num13);
                        vector[num55] = num51;
                        num47 = num12 / num51;
                        num9 = num13 / num51;
                        num12 = (num53 * num47) + (b * num9);
                        b = (b * num47) - (num53 * num9);
                        num13 = num50 * num9;
                        num50 *= num47;
                        for (var num57 = 0; num57 < columns; num57++)
                        {
                            num53 = v[num57, num55];
                            num51 = v[num57, num56];
                            v[num57, num55] = (num53 * num47) + (num51 * num9);
                            v[num57, num56] = (num51 * num47) - (num53 * num9);
                        }
                        num51 = pythag(num12, num13);
                        w[num55] = num51;
                        if (num51 != 0.0)
                        {
                            num51 = 1.0 / num51;
                            num47 = num12 * num51;
                            num9 = num13 * num51;
                        }
                        num12 = (num47 * b) + (num9 * num50);
                        num53 = (num47 * num50) - (num9 * b);
                        for (var num58 = 0; num58 < rows; num58++)
                        {
                            num50 = u[num58, num55];
                            num51 = u[num58, num56];
                            u[num58, num55] = (num50 * num47) + (num51 * num9);
                            u[num58, num56] = (num51 * num47) - (num50 * num9);
                        }
                    }
                    vector[num3] = 0.0;
                    vector[m] = num12;
                    w[m] = num53;
                }
            }
        }

        private  double pythag(double a, double b)
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

        public double ConditionNumber
        {
            get
            {
                double positiveInfinity = double.PositiveInfinity;
                double num2 = 0.0;
                for (var i = 0; i < W.Rows; i++)
                {
                    positiveInfinity = Math.Min(positiveInfinity, w[i]);
                    num2 = Math.Max(num2, w[i]);
                }
                return (num2 / positiveInfinity);
            }
        }

        public int LargestSingularIndex
        {
            get
            {
                double num = W[0];
                int num2 = 0;
                for (var i = 1; i < W.Size; i++)
                {
                    if (W[i] > num)
                    {
                        num = W[i];
                        num2 = i;
                    }
                }
                return num2;
            }
        }

        public int SmallestSingularIndex
        {
            get
            {
                double num = W[0];
                int num2 = 0;
                for (var i = 1; i < W.Size; i++)
                {
                    if (W[i] < num)
                    {
                        num = W[i];
                        num2 = i;
                    }
                }
                return num2;
            }
        }

        public Matrix U
        {
            get
            {
                return u;
            }
        }

        public Matrix UT
        {
            get
            {
                return U.Transpose();
            }
        }

        public SquareMatrix V
        {
            get
            {
                return v;
            }
        }

        public SquareMatrix VT
        {
            get
            {
                return V.Transpose();
            }
        }

        public Vector W
        {
            get
            {
                return w;
            }
        }
    }
}

