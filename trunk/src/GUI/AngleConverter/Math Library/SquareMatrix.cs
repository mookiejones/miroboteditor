namespace ISTUK.MathLibrary
{
    using System;

    [Serializable]
    public class SquareMatrix : Matrix
    {
        public SquareMatrix() : base(0, 0)
        {
        }

        public SquareMatrix(Matrix mat)
        {
            if (mat.Rows != mat.Columns)
            {
                throw new MatrixException("Matrix is not square. Cannot cast to a SquareMatrix");
            }
            Size = mat.Rows;
            for (var i = 0; i < base.Rows; i++)
            {
                for (var j = 0; j < base.Columns; j++)
                {
                    base[i, j] = mat[i, j];
                }
            }
        }

        public SquareMatrix(SquareMatrix mat) : base(mat)
        {
        }

        public SquareMatrix(int size) : base(size, size)
        {
        }

        public SquareMatrix(int size, params double[] elements) : base(size, size, elements)
        {
        }

        public double Determinant()
        {
            SquareMatrix matrix = new SquareMatrix(this);
            double num = matrix.MakeRowEchelon();
            for (var i = 0; i < Size; i++)
            {
                if (matrix.IsRowZero(i))
                {
                    return 0.0;
                }
            }
            return num;
        }

        public static SquareMatrix Identity(int size)
        {
            SquareMatrix matrix = new SquareMatrix(size);
            for (var i = 0; i < size; i++)
            {
                matrix[i, i] = 1.0;
            }
            return matrix;
        }

        public SquareMatrix Inverse()
        {
            Matrix matrix = base.Augment(Identity(Size));
            matrix.MakeRowEchelon();
            SquareMatrix matrix2 = new SquareMatrix(Size);
            SquareMatrix matrix3 = new SquareMatrix(Size);
            for (var i = 0; i < Size; i++)
            {
                matrix2.SetColumn(i, matrix.GetColumn(i));
                matrix3.SetColumn(i, matrix.GetColumn(i + Size));
            }
            for (var j = 0; j < matrix2.Rows; j++)
            {
                if (matrix2.IsRowZero(j))
                {
                    throw new MatrixException("Matrix is singular");
                }
            }
            for (var k = Size - 1; k > 0; k--)
            {
                for (var m = k - 1; m >= 0; m--)
                {
                    double scalar = -matrix2[m, k];
                    matrix2.AddRowTimesScalar(m, k, scalar);
                    matrix3.AddRowTimesScalar(m, k, scalar);
                }
            }
            return matrix3;
        }

        public bool IsRotationMatrix()
        {
            if (!base.IsNaN())
            {
                if (Math.Abs((double) (Determinant() - 1.0)) > 0.001)
                {
                    return false;
                }
                Matrix matrix = Inverse();
                Matrix matrix2 = Transpose();
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        if (Math.Abs((double) (matrix[i, j] - matrix2[i, j])) > 0.001)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void LUDecomposition(out SquareMatrix L, out SquareMatrix U)
        {
            L = new SquareMatrix(Size);
            U = new SquareMatrix(Size);
            if (base[0, 0] == 0.0)
            {
                throw new MatrixException("Unable to decompose matrix");
            }
            L.SetColumn(0, base.GetColumn(0));
            U.SetRow(0, base.GetRow(0));
            U.MultiplyRow(0, 1.0 / base[0, 0]);
            for (var i = 1; i < Size; i++)
            {
                Vector[] vectorArray = new Vector[Size];
                Vector[] vectorArray2 = new Vector[Size];
                for (var j = 1; j < Size; j++)
                {
                    vectorArray[j] = new Vector(i);
                    vectorArray2[j] = new Vector(i);
                    Vector row = L.GetRow(j);
                    Vector column = U.GetColumn(j);
                    for (var m = 0; m < i; m++)
                    {
                        vectorArray[j][m] = row[m];
                        vectorArray2[j][m] = column[m];
                    }
                }
                for (var k = i; k < Size; k++)
                {
                    L[k, i] = base[k, i] - Vector.Dot(vectorArray[k], vectorArray2[i]);
                    if (k == i)
                    {
                        U[i, k] = 1.0;
                    }
                    else
                    {
                        if (L[i, i] == 0.0)
                        {
                            throw new MatrixException("Unable to decompose matrix");
                        }
                        U[i, k] = (base[i, k] - Vector.Dot(vectorArray[i], vectorArray2[k])) / L[i, i];
                    }
                }
            }
        }

        public SquareMatrix Minor(int i, int j)
        {
            SquareMatrix matrix = new SquareMatrix(Size - 1);
            int num = 0;
            for (var k = 0; k < base.Rows; k++)
            {
                if (k != i)
                {
                    int num3 = 0;
                    for (var m = 0; m < base.Columns; m++)
                    {
                        if (m != j)
                        {
                            matrix[num, num3] = base[k, m];
                            num3++;
                        }
                    }
                    num++;
                }
            }
            return matrix;
        }

        public static SquareMatrix NaN(int size)
        {
            return new SquareMatrix(Matrix.NaN(size, size));
        }

        //public static SquareMatrix operator *(SquareMatrix lhs, SquareMatrix rhs)
        //{
        //    try
        //    {
        //        return new SquareMatrix(lhs * rhs);
        //    }
        //    catch (StackOverflowException ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        return new SquareMatrix();
        //    }
        //}

        public SquareMatrix Power(int power)
        {
            Matrix mat = new SquareMatrix(this);
            for (var i = 1; i < power; i++)
            {
                mat = this * mat;
            }
            return new SquareMatrix(mat);
        }

        public double Trace()
        {
            double num = 0.0;
            for (var i = 0; i < Size; i++)
            {
                num += base[i, i];
            }
            return num;
        }

        public new SquareMatrix Transpose()
        {
            return new SquareMatrix(base.Transpose());
        }

        public int Size
        {
            get
            {
                return base.Rows;
            }
            set
            {
                base.SetSize(value, value);
            }
        }
    }
}

