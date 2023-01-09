using System;

namespace AngleConverterControl.Model
{
    public sealed class SVD
    {
        private const double EPSILON = 0.0001;

        public SVD(Matrix mat)
        {
            int i = 0;
            if (mat.Rows < mat.Columns)
            {
                throw new MatrixException("Matrix must have rows >= columns");
            }
            int rows = mat.Rows;
            int columns = mat.Columns;
            Vector vector = new Vector(columns);
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            int num4 = 0;
            U = new Matrix(mat);
            V = new SquareMatrix(columns);
            W = new Vector(columns);
            for (int j = 0; j < columns; j++)
            {
                i = j + 1;
                vector[j] = num * num2;
                double num5 = num2 = num = 0.0;
                if (j < rows)
                {
                    for (int k = j; k < rows; k++)
                    {
                        num += Math.Abs(U[k, j]);
                    }
                    if (Math.Abs(num - 0.0) > 0.0001)
                    {
                        for (int l = j; l < rows; l++)
                        {
                            Matrix u;
                            int row;
                            int column;
                            (u = U)[row = l, column = j] = u[row, column] / num;
                            num5 += U[l, j] * U[l, j];
                        }
                        double num6 = U[j, j];
                        num2 = (num6 < 0.0) ? Math.Sqrt(num5) : (-Math.Sqrt(num5));
                        double num7 = (num6 * num2) - num5;
                        U[j, j] = num6 - num2;
                        if (j != columns - 1)
                        {
                            for (int m = i; m < columns; m++)
                            {
                                num5 = 0.0;
                                for (int n = j; n < rows; n++)
                                {
                                    num5 += U[n, j] * U[n, m];
                                }
                                num6 = num5 / num7;
                                for (int num8 = j; num8 < rows; num8++)
                                {
                                    Matrix u2;
                                    int num9;
                                    int column2;
                                    (u2 = U)[num9 = num8, column2 = m] = u2[num9, column2] + (num6 * U[num8, j]);
                                }
                            }
                        }
                        for (int num10 = j; num10 < rows; num10++)
                        {
                            Matrix u;
                            int row;
                            int column;
                            (u = U)[row = num10, column = j] = u[row, column] * num;
                        }
                    }
                }
                W[j] = num * num2;
                num5 = num2 = num = 0.0;
                if (j < rows && j != columns - 1)
                {
                    for (int num11 = i; num11 < columns; num11++)
                    {
                        num += Math.Abs(U[j, num11]);
                    }
                    if (Math.Abs(num - 0.0) > 0.0001)
                    {
                        for (int num12 = i; num12 < columns; num12++)
                        {
                            Matrix u;
                            int row;
                            int column;
                            (u = U)[row = j, column = num12] = u[row, column] / num;
                            num5 += U[j, num12] * U[j, num12];
                        }
                        double num6 = U[j, i];
                        num2 = (num6 < 0.0) ? Math.Sqrt(num5) : (-Math.Sqrt(num5));
                        double num7 = (num6 * num2) - num5;
                        U[j, i] = num6 - num2;
                        for (int num13 = i; num13 < columns; num13++)
                        {
                            vector[num13] = U[j, num13] / num7;
                        }
                        if (j != rows - 1)
                        {
                            for (int num14 = i; num14 < rows; num14++)
                            {
                                num5 = 0.0;
                                for (int num15 = i; num15 < columns; num15++)
                                {
                                    num5 += U[num14, num15] * U[j, num15];
                                }
                                for (int num16 = i; num16 < columns; num16++)
                                {
                                    Matrix u3;
                                    int row2;
                                    int column3;
                                    (u3 = U)[row2 = num14, column3 = num16] = u3[row2, column3] + (num5 * vector[num16]);
                                }
                            }
                        }
                        for (int num17 = i; num17 < columns; num17++)
                        {
                            Matrix u;
                            int row;
                            int column;
                            (u = U)[row = j, column = num17] = u[row, column] * num;
                        }
                    }
                }
                num3 = Math.Max(num3, Math.Abs(W[j]) + Math.Abs(vector[j]));
            }
            for (int num18 = columns - 1; num18 >= 0; num18--)
            {
                if (num18 < columns - 1)
                {
                    if (Math.Abs(num2 - 0.0) > 0.0001)
                    {
                        for (int num19 = i; num19 < columns; num19++)
                        {
                            V[num19, num18] = U[num18, num19] / U[num18, i] / num2;
                        }
                        for (int num20 = i; num20 < columns; num20++)
                        {
                            double num5 = 0.0;
                            for (int num21 = i; num21 < columns; num21++)
                            {
                                num5 += U[num18, num21] * V[num21, num20];
                            }
                            for (int num22 = i; num22 < columns; num22++)
                            {
                                int num9;
                                Matrix v;
                                int row3;
                                (v = V)[row3 = num22, num9 = num20] = v[row3, num9] + (num5 * V[num22, num18]);
                            }
                        }
                    }
                    for (int num23 = i; num23 < columns; num23++)
                    {
                        V[num18, num23] = V[num23, num18] = 0.0;
                    }
                }
                V[num18, num18] = 1.0;
                num2 = vector[num18];
                i = num18;
            }
            for (int num24 = columns - 1; num24 >= 0; num24--)
            {
                i = num24 + 1;
                num2 = W[num24];
                if (num24 < columns - 1)
                {
                    for (int num25 = i; num25 < columns; num25++)
                    {
                        U[num24, num25] = 0.0;
                    }
                }
                Matrix u2;
                int num9;
                int column2;
                if (Math.Abs(num2 - 0.0) > 0.0001)
                {
                    num2 = 1.0 / num2;
                    if (num24 != columns - 1)
                    {
                        for (int num26 = i; num26 < columns; num26++)
                        {
                            double num5 = 0.0;
                            for (int num27 = i; num27 < rows; num27++)
                            {
                                num5 += U[num27, num24] * U[num27, num26];
                            }
                            double num6 = num5 / U[num24, num24] * num2;
                            for (int num28 = num24; num28 < rows; num28++)
                            {
                                (u2 = U)[num9 = num28, column2 = num26] = u2[num9, column2] + (num6 * U[num28, num24]);
                            }
                        }
                    }
                    for (int num29 = num24; num29 < rows; num29++)
                    {
                        Matrix u;
                        int row;
                        int column;
                        (u = U)[row = num29, column = num24] = u[row, column] * num2;
                    }
                }
                else
                {
                    for (int num30 = num24; num30 < rows; num30++)
                    {
                        U[num30, num24] = 0.0;
                    }
                }
                (u2 = U)[num9 = num24, column2 = num24] = u2[num9, column2] + 1.0;
            }
            for (int num31 = columns - 1; num31 >= 0; num31--)
            {
                for (int num32 = 0; num32 < 30; num32++)
                {
                    int num33 = 1;
                    for (i = num31; i >= 0; i--)
                    {
                        num4 = i - 1;
                        if (Math.Abs(Math.Abs(vector[i]) + num3 - num3) < 0.0001)
                        {
                            num33 = 0;
                            break;
                        }
                        if (Math.Abs(Math.Abs(W[num4]) + num3 - num3) < 0.0001)
                        {
                            break;
                        }
                    }
                    double num5;
                    double num6;
                    double num7;
                    double num35;
                    double num37;
                    double num38;
                    if (num33 != 0)
                    {
                        num5 = 1.0;
                        for (int num34 = i; num34 <= num31; num34++)
                        {
                            num6 = num5 * vector[num34];
                            if (Math.Abs(Math.Abs(num6) + num3 - num3) > 0.0001)
                            {
                                num2 = W[num34];
                                num7 = Pythag(num6, num2);
                                W[num34] = (float)num7;
                                num7 = 1.0 / num7;
                                num35 = num2 * num7;
                                num5 = -num6 * num7;
                                for (int num36 = 0; num36 < rows; num36++)
                                {
                                    num37 = U[num36, num4];
                                    num38 = U[num36, num34];
                                    U[num36, num4] = (num37 * num35) + (num38 * num5);
                                    U[num36, num34] = (num38 * num35) - (num37 * num5);
                                }
                            }
                        }
                    }
                    num38 = W[num31];
                    if (i == num31)
                    {
                        if (num38 < 0.0)
                        {
                            W[num31] = -num38;
                            for (int num39 = 0; num39 < columns; num39++)
                            {
                                V[num39, num31] = -V[num39, num31];
                            }
                        }
                        break;
                    }
                    if (num32 >= 30)
                    {
                        throw new MatrixException("No convergence after 30 iterations");
                    }
                    double num40 = W[i];
                    num4 = num31 - 1;
                    num37 = W[num4];
                    num2 = vector[num4];
                    num7 = vector[num31];
                    num6 = (((num37 - num38) * (num37 + num38)) + ((num2 - num7) * (num2 + num7))) / (2.0 * num7 * num37);
                    num2 = Pythag(num6, 1.0);
                    double num41 = (num6 >= 0.0) ? num2 : (-num2);
                    num6 = (((num40 - num38) * (num40 + num38)) + (num7 * ((num37 / (num6 + num41)) - num7))) / num40;
                    num5 = num35 = 1.0;
                    for (int num42 = i; num42 <= num4; num42++)
                    {
                        int num43 = num42 + 1;
                        num2 = vector[num43];
                        num37 = W[num43];
                        num7 = num5 * num2;
                        num2 = num35 * num2;
                        num38 = Pythag(num6, num7);
                        vector[num42] = num38;
                        num35 = num6 / num38;
                        num5 = num7 / num38;
                        num6 = (num40 * num35) + (num2 * num5);
                        num2 = (num2 * num35) - (num40 * num5);
                        num7 = num37 * num5;
                        num37 *= num35;
                        for (int num44 = 0; num44 < columns; num44++)
                        {
                            num40 = V[num44, num42];
                            num38 = V[num44, num43];
                            V[num44, num42] = (num40 * num35) + (num38 * num5);
                            V[num44, num43] = (num38 * num35) - (num40 * num5);
                        }
                        num38 = Pythag(num6, num7);
                        W[num42] = num38;
                        if (Math.Abs(num38 - 0.0) > 0.0001)
                        {
                            num38 = 1.0 / num38;
                            num35 = num6 * num38;
                            num5 = num7 * num38;
                        }
                        num6 = (num35 * num2) + (num5 * num37);
                        num40 = (num35 * num37) - (num5 * num2);
                        for (int num45 = 0; num45 < rows; num45++)
                        {
                            num37 = U[num45, num42];
                            num38 = U[num45, num43];
                            U[num45, num42] = (num37 * num35) + (num38 * num5);
                            U[num45, num43] = (num38 * num35) - (num37 * num5);
                        }
                    }
                    vector[i] = 0.0;
                    vector[num31] = num6;
                    W[num31] = num40;
                }
            }
        }

        public double ConditionNumber
        {
            get
            {
                double num = double.PositiveInfinity;
                double num2 = 0.0;
                for (int i = 0; i < W.Rows; i++)
                {
                    num = Math.Min(num, W[i]);
                    num2 = Math.Max(num2, W[i]);
                }
                return num2 / num;
            }
        }

        public int SmallestSingularIndex
        {
            get
            {
                double num = W[0];
                int result = 0;
                for (int i = 1; i < W.Size; i++)
                {
                    if (W[i] < num)
                    {
                        num = W[i];
                        result = i;
                    }
                }
                return result;
            }
        }

        public Matrix U { get; }

        public SquareMatrix V { get; }

        public Vector W { get; }

        private static double Pythag(double a, double b)
        {
            double num = Math.Abs(a);
            double num2 = Math.Abs(b);
            double result;
            if (num > num2)
            {
                double num3 = num2 / num;
                result = num * Math.Sqrt(1.0 + (num3 * num3));
            }
            else
            {
                if (num2 > 0.0)
                {
                    double num3 = num / num2;
                    result = num2 * Math.Sqrt(1.0 + (num3 * num3));
                }
                else
                {
                    result = 0.0;
                }
            }
            return result;
        }
    }
}
