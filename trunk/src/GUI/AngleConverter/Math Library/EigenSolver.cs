namespace ISTUK.MathLibrary
{
    using System;

    public class EigenSolver
    {
        private readonly Vector Diag;
        private bool IsRotation;
        private SquareMatrix mat;
        private readonly Vector Subd;

        public EigenSolver(SquareMatrix m)
        {
            if (m.Size != 3)
            {
                throw new MatrixException("Unable to solve a matrix of size != 3");
            }
            mat = new SquareMatrix(m);
            Diag = new Vector(3);
            Subd = new Vector(3);
        }

        public static double[] ComputeRoots(Matrix m)
        {
            double num = m[0, 0];
            double num2 = m[0, 1];
            double num3 = m[0, 2];
            double num4 = m[1, 1];
            double num5 = m[1, 2];
            double num6 = m[2, 2];
            double num7 = (((((num * num4) * num6) + (((2.0 * num2) * num3) * num5)) - ((num * num5) * num5)) - ((num4 * num3) * num3)) - ((num6 * num2) * num2);
            double num8 = (((((num * num4) - (num2 * num2)) + (num * num6)) - (num3 * num3)) + (num4 * num6)) - (num5 * num5);
            double num9 = (num + num4) + num6;
            double num10 = num9 / 3.0;
            double num11 = (num8 - (num9 / num10)) / 3.0;
            if (num11 > 0.0)
            {
                num11 = 0.0;
            }
            double x = 0.5 * (num7 + (num10 * (((2.0 * num10) * num10) - num8)));
            double num13 = (x * x) + ((num11 * num11) * num11);
            if (num13 > 0.0)
            {
                num13 = 0.0;
            }
            double num14 = Math.Sqrt(-num11);
            double d = Math.Atan2(Math.Sqrt(-num13), x) / 3.0;
            double num16 = Math.Cos(d);
            double num17 = Math.Sin(d);
            return new double[] { (num10 + ((2.0 * num14) * num16)), (num10 - (num14 * (num16 + (Math.Sqrt(3.0) * num17)))), (num10 - (num14 * (num16 - (Math.Sqrt(3.0) * num17)))) };
        }

        private void DecreasingSort()
        {
            for (var i = 0; i <= 1; i++)
            {
                int num2 = i;
                double num3 = Diag[num2];
                int num4 = i + 1;
                while (num4 < 3)
                {
                    if (Diag[num4] > num3)
                    {
                        num2 = num4;
                        num3 = Diag[num2];
                    }
                    num4++;
                }
                if (num2 != i)
                {
                    Diag[num2] = Diag[i];
                    Diag[i] = num3;
                    for (num4 = 0; num4 < 3; num4++)
                    {
                        double num5 = mat[num4, i];
                        mat[num4, i] = mat[num4, num2];
                        mat[num4, num2] = num5;
                        IsRotation = !IsRotation;
                    }
                }
            }
        }

        public void DecrSortEigenStuff()
        {
            Tridiagonal();
            QLAlgorithm();
            DecreasingSort();
            GuaranteeRotation();
        }

        private void GuaranteeRotation()
        {
            if (!IsRotation)
            {
                for (var i = 0; i < 3; i++)
                {
                    mat[i, 0] = -mat[i, 0];
                }
            }
        }

        private void QLAlgorithm()
        {
            for (var i = 0; i < 3; i++)
            {
                int num2 = 0;
                while (num2 < 0x20)
                {
                    Vector vector;
                    int num14;
                    int num3 = i;
                    while (num3 <= 1)
                    {
                        double num4 = Math.Abs(Diag[num3]) + Math.Abs(Diag[num3 + 1]);
                        if ((Math.Abs(Subd[num3]) + num4) == num4)
                        {
                            break;
                        }
                        num3++;
                    }
                    if (num3 == i)
                    {
                        break;
                    }
                    double num5 = (Diag[i + 1] - Diag[i]) / (2.0 * Subd[i]);
                    double num6 = Math.Sqrt((num5 * num5) + 1.0);
                    if (num5 < 0.0)
                    {
                        num5 = (Diag[num3] - Diag[i]) + (Subd[i] / (num5 - num6));
                    }
                    else
                    {
                        num5 = (Diag[num3] - Diag[i]) + (Subd[i] / (num5 + num6));
                    }
                    double num7 = 1.0;
                    double num8 = 1.0;
                    double num9 = 0.0;
                    for (var j = num3 - 1; j >= i; j--)
                    {
                        double num11 = num7 * Subd[j];
                        double num12 = num8 * Subd[j];
                        if (Math.Abs(num11) >= Math.Abs(num5))
                        {
                            num8 = num5 / num11;
                            num6 = Math.Sqrt((num8 * num8) + 1.0);
                            Subd[j + 1] = num11 * num6;
                            num7 = 1.0 / num6;
                            num8 *= num7;
                        }
                        else
                        {
                            num7 = num11 / num5;
                            num6 = Math.Sqrt((num7 * num7) + 1.0);
                            Subd[j + 1] = num5 * num6;
                            num8 = 1.0 / num6;
                            num7 *= num8;
                        }
                        num5 = Diag[j + 1] - num9;
                        num6 = ((Diag[j] - num5) * num7) + ((2.0 * num12) * num8);
                        num9 = num7 * num6;
                        Diag[j + 1] = num5 + num9;
                        num5 = (num8 * num6) - num12;
                        for (var k = 0; k < 3; k++)
                        {
                            num11 = mat[k, j + 1];
                            mat[k, j + 1] = (num7 * mat[k, j]) + (num8 * num11);
                            mat[k, j] = (num8 * mat[k, j]) - (num7 * num11);
                        }
                    }
                    (vector = Diag)[num14 = i] = vector[num14] - num9;
                    Subd[i] = num5;
                    Subd[num3] = 0.0;
                    num2++;
                }
                if (num2 == 0x20)
                {
                    throw new MatrixException("No Convergence after 10 iterations");
                }
            }
        }

        private void Tridiagonal()
        {
            Matrix matrix = new SquareMatrix(3);
            matrix.SetRow(0, mat.GetRow(0));
            matrix.SetRow(1, mat.GetRow(1));
            Diag[0] = matrix[0, 0];
            Subd[2] = 0.0;
            if (matrix[0, 2] != 0.0)
            {
                Matrix matrix2;
                Matrix matrix3;
                double num = Math.Sqrt((matrix[0, 1] * matrix[0, 1]) + (matrix[0, 2] * matrix[0, 2]));
                double num2 = 1.0 / num;
                (matrix2 = matrix)[0, 1] = matrix2[0, 1] * num2;
                (matrix3 = matrix)[0, 2] = matrix3[0, 2] * num2;
                double num3 = ((2.0 * matrix[0, 1]) * matrix[1, 2]) + (matrix[0, 2] * (matrix[2, 2] - matrix[1, 1]));
                Diag[1] = matrix[1, 1] + (matrix[0, 2] * num3);
                Diag[2] = matrix[2, 2] - (matrix[0, 2] * num3);
                Subd[0] = num;
                Subd[1] = matrix[1, 2] - (matrix[0, 1] * num3);
                mat = SquareMatrix.Identity(3);
                mat[1, 1] = matrix[0, 1];
                mat[1, 2] = matrix[0, 2];
                mat[2, 1] = matrix[0, 2];
                mat[2, 2] = -matrix[0, 1];
                IsRotation = false;
            }
            else
            {
                Diag[1] = matrix[1, 1];
                Diag[2] = matrix[2, 2];
                Subd[0] = matrix[0, 1];
                Subd[1] = matrix[1, 2];
                mat = SquareMatrix.Identity(3);
                IsRotation = true;
            }
        }
    }
}

