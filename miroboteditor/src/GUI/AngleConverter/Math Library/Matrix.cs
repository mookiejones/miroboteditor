namespace ISTUK.MathLibrary
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;

    [Serializable]
    public class Matrix : IFormattable
    {
        private int columns;
        private double[,] elements;
        private int rows;

        public Matrix()
        {
            SetSize(0, 0);
        }

        public Matrix(Collection<Point3D> points)
        {
            SetSize(points.Count, 3);
            for (var i = 0; i < points.Count; i++)
            {
                SetRow(i, (Vector3D) points[i]);
            }
        }

        public Matrix(Matrix mat)
        {
            SetSize(mat.Rows, mat.Columns);
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    elements[i, j] = mat[i,j];
                }
            }
        }

        public Matrix(string str)
        {
            int startIndex = str.IndexOf('[') + 1;
            int num2 = str.LastIndexOf(']');
            bool flag = false;
            if (str.Substring(num2 + 1).Trim().StartsWith("'"))
            {
                flag = true;
            }
            string[] strArray = str.Substring(startIndex, num2 - startIndex).Split(new char[] { ';' });
            string[] strArray2 = strArray[0].Split(new char[] { ',' });
            int length = strArray.Length;
            int rows = strArray2.Length;
            if ((length <= 0) || (rows <= 0))
            {
                throw new MatrixException("String does not contain a valid matrix");
            }
            if (flag)
            {
                SetSize(rows, length);
                for (var i = 0; i < Columns; i++)
                {
                    string[] strArray3 = strArray[i].Split(new char[] { ',' });
                    if (strArray3.Length != length)
                    {
                        throw new MatrixException("Matrix does not contain columns that are of equal lengths");
                    }
                    for (var j = 0; j < length; j++)
                    {
                        string str3 = strArray3[j];
                        try
                        {
                            this[i, j] = Convert.ToDouble(str3, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            throw new MatrixException("Invalid format in matrix string");
                        }
                    }
                }
            }
            else
            {
                SetSize(length, rows);
                for (int k = 0; k < Rows; k++)
                {
                    string[] strArray4 = strArray[k].Split(new char[] { ',' });
                    if (strArray4.Length != rows)
                    {
                        throw new MatrixException("Matrix does not contain rows that are of equal lengths");
                    }
                    for (int m = 0; m < rows; m++)
                    {
                        string str4 = strArray4[m];
                        try
                        {
                            this[k, m] = Convert.ToDouble(str4);
                        }
                        catch
                        {
                            throw new MatrixException("Invalid format in matrix string");
                        }
                    }
                }
            }
        }

        public Matrix(Collection<Vector> vectors, MatrixDirection direction)
        {
            int num2;
            switch (direction)
            {
                case MatrixDirection.RowWise:
                    SetSize(vectors.Count, vectors[0].Size);
                    for (var i = 0; i < vectors.Count; i++)
                    {
                        if (vectors[i].Size != vectors[0].Size)
                        {
                            throw new MatrixException("List contains vectors of differing sizes");
                        }
                        SetRow(i, vectors[i]);
                    }
                    return;

                case MatrixDirection.ColumnWise:
                    SetSize(vectors[0].Size, vectors.Count);
                    num2 = 0;
                    break;

                default:
                    return;
            }
            while (num2 < vectors.Count)
            {
                if (vectors[num2].Size != vectors[0].Size)
                {
                    throw new MatrixException("List contains vectors of differing sizes");
                }
                SetColumn(num2, vectors[num2]);
                num2++;
            }
        }

        public Matrix(int rows, int columns)
        {
            SetSize(rows, columns);
        }

        public Matrix(int rows, int columns, params double[] elements)
        {
            if (elements.Length != (rows * columns))
            {
                throw new MatrixException("Number of elements does not match matrix dimension");
            }
            SetSize(rows, columns);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    this.elements[i,j] = elements[j + (i * columns)];
                }
            }
        }

        public void AddColumn(int column1, int column2)
        {
            Vector column = GetColumn(column2);
            for (var i = 0; i < Rows; i++)
            {
                Matrix matrix;
                int num2;
                int num3;
                (matrix = this)[num2 = i, num3 = column1] = matrix[num2, num3] + column[i];
            }
        }

        public void AddColumnTimesScalar(int column1, int column2, double scalar)
        {
            Vector column = GetColumn(column2);
            for (var i = 0; i < Rows; i++)
            {
                Matrix matrix;
                int num2;
                int num3;
                (matrix = this)[num2 = i, num3 = column1] = matrix[num2, num3] + (scalar * column[i]);
            }
        }

        public void AddRow(int row1, int row2)
        {
            Vector row = GetRow(row2);
            for (var i = 0; i < Columns; i++)
            {
                Matrix matrix;
                int num2;
                int num3;
                (matrix = this)[num2 = row1, num3 = i] = matrix[num2, num3] + row[0, i];
            }
        }

        public void AddRowTimesScalar(int row1, int row2, double scalar)
        {
            Vector row = GetRow(row2);
            for (var i = 0; i < Columns; i++)
            {
                Matrix matrix;
                int num2;
                int num3;
                (matrix = this)[num2 = row1, num3 = i] = matrix[num2, num3] + (scalar * row[i, 0]);
            }
        }

        public Matrix Augment(Matrix mat)
        {
            if (Rows != mat.Rows)
            {
                throw new MatrixException("Cannot augment matrices with different number of rows");
            }
            Matrix matrix = new Matrix(Rows, Columns + mat.Columns);
            for (var i = 0; i < Columns; i++)
            {
                matrix.SetColumn(i, GetColumn(i));
            }
            for (var j = 0; j < mat.Columns; j++)
            {
                matrix.SetColumn(j + Columns, mat.GetColumn(j));
            }
            return matrix;
        }

        public double ConditionNumber()
        {
            ISTUK.MathLibrary.SVD svd = new ISTUK.MathLibrary.SVD(this);
            return svd.ConditionNumber;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public Vector GetColumn(int column)
        {
            Vector vector = new Vector(Rows);
            for (var i = 0; i < Rows; i++)
            {
                vector[i] = this[i, column];
            }
            return vector;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Vector GetRow(int row)
        {
            Vector vector = new Vector(Columns);
            for (var i = 0; i < Columns; i++)
            {
                vector[i] = this[row, i];
            }
            return vector;
        }

        public bool IsColumnZero(int column)
        {
            for (var i = 0; i < Rows; i++)
            {
                if (Math.Abs(this[i, column]) > 1E-08)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsColumnZeroBelowRow(int column, int row)
        {
            for (var i = row; i < Rows; i++)
            {
                if (Math.Abs(this[i, column]) > 1E-08)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsNaN()
        {
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    if (!double.IsNaN(this[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsRowEchelon()
        {
            bool flag = false;
            int columns = -1;
            for (var i = 0; i < Rows; i++)
            {
                if (flag && !IsRowZero(i))
                {
                    return false;
                }
                if (IsRowZero(i))
                {
                    flag = true;
                    columns = Columns;
                }
                else
                {
                    for (var j = 0; j < Columns; j++)
                    {
                        if (this[i, j] != 0.0)
                        {
                            if (j <= columns)
                            {
                                return false;
                            }
                            if (this[i, j] != 1.0)
                            {
                                return false;
                            }
                            columns = j;
                            break;
                        }
                    }
                }
            }
            return true;
        }

        public bool IsRowZero(int row)
        {
            for (var i = 0; i < Columns; i++)
            {
                if (Math.Abs(this[row, i]) > 1E-08)
                {
                    return false;
                }
            }
            return true;
        }

        public double MakeRowEchelon()
        {
            double num = 1.0;
            for (var i = 0; i < Rows; i++)
            {
                int num7;
                int num3 = -1;
                for (var j = 0; j < Columns; j++)
                {
                    if (!IsColumnZeroBelowRow(j, i))
                    {
                        num3 = j;
                        break;
                    }
                }
                if (num3 == -1)
                {
                    return num;
                }
                double naN = double.NaN;
                for (int k = i; k < Rows; k++)
                {
                    if (Math.Abs(this[k, num3]) > 1E-05)
                    {
                        if (i != k)
                        {
                            SwapRows(i, k);
                            num = -num;
                        }
                        naN = this[i, num3];
                        break;
                    }
                }
                if (naN != 1.0)
                {
                    MultiplyRow(i, 1.0 / naN);
                    num *= naN;
                }
            Label_00CE:
                num7 = -1;
                double num8 = double.NaN;
                for (int m = i + 1; m < Rows; m++)
                {
                    if (this[m, num3] != 0.0)
                    {
                        num7 = m;
                        num8 = this[m, num3];
                        break;
                    }
                }
                if (num7 != -1)
                {
                    AddRowTimesScalar(num7, i, -num8);
                    goto Label_00CE;
                }
            }
            return num;
        }

        public void MultiplyColumn(int column, double scalar)
        {
            for (var i = 0; i < Rows; i++)
            {
                Matrix matrix;
                int num2;
                int num3;
                (matrix = this)[num2 = i, num3 = column] = matrix[num2, num3] * scalar;
            }
        }

        public void MultiplyRow(int row, double scalar)
        {
            for (var i = 0; i < Columns; i++)
            {
                Matrix matrix;
                int num2;
                int num3;
                (matrix = this)[num2 = row, num3 = i] = matrix[num2, num3] * scalar;
            }
        }

        public static Matrix NaN(int rows, int columns)
        {
            Matrix matrix = new Matrix(rows, columns);
            matrix.SetSize(rows, columns);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    matrix[i, j] = double.NaN;
                }
            }
            return matrix;
        }

        public static Matrix operator +(Matrix lhs, Matrix rhs)
        {
            if ((lhs.Rows != rhs.Rows) || (lhs.Columns != rhs.Columns))
            {
                throw new MatrixException("Matrices are not the same size");
            }
            Matrix matrix = new Matrix(lhs.Rows, lhs.Columns);
            for (var i = 0; i < lhs.Rows; i++)
            {
                for (var j = 0; j < lhs.Columns; j++)
                {
                    matrix[i, j] = lhs[i, j] + rhs[i, j];
                }
            }
            return matrix;
        }
        public static Matrix Add(Matrix lhs, Matrix rhs)
        {
            if ((lhs.Rows != rhs.Rows) || (lhs.Columns != rhs.Columns))
            {
                throw new MatrixException("Matrices are not the same size");
            }
            Matrix matrix = new Matrix(lhs.Rows, lhs.Columns);
            for (var i = 0; i < lhs.Rows; i++)
            {
                for (var j = 0; j < lhs.Columns; j++)
                {
                    matrix[i, j] = lhs[i, j] + rhs[i, j];
                }
            }
            return matrix;
        }

        public static Matrix operator +(Matrix mat, double scalar)
        {
            Matrix matrix = new Matrix(mat);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    Matrix matrix2;
                    int num3;
                    int num4;
                    (matrix2 = matrix)[num3 = i, num4 = j] = matrix2[num3, num4] + scalar;
                }
            }
            return matrix;
        }
        public static Matrix Add(Matrix mat, double scalar)
        {
            Matrix matrix = new Matrix(mat);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    Matrix matrix2;
                    int num3;
                    int num4;
                    (matrix2 = matrix)[num3 = i, num4 = j] = matrix2[num3, num4] + scalar;
                }
            }
            return matrix;
        }
        public static Matrix operator +(double scalar, Matrix mat)
        {
            return new Matrix();
            //return (mat + ((Matrix) scalar));
        }
        public static Matrix Divide(Matrix mat, double scalar)
        {
            if (scalar == 0.0)
            {
                throw new DivideByZeroException();
            }
            return (Matrix)(mat * (1.0 / scalar));
        }
        public static Matrix operator /(Matrix mat, double scalar)
        {
            if (scalar == 0.0)
            {
                throw new DivideByZeroException();
            }
            return (Matrix) (mat * (1.0 / scalar));
        }

        public static bool operator ==(Matrix m1, Matrix m2)
        {
            try
            {
                if ((m1.Rows != m2.Rows) || (m1.Columns != m2.Columns))
                {
                    return false;
                }
                for (var i = 0; i < m1.Rows; i++)
                {
                    for (var j = 0; j < m1.Columns; j++)
                    {
                        if (m1[i, j] != m2[i, j])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator !=(Matrix m1, Matrix m2)
        {
            return !(m1 == m2);
        }

        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            if (lhs.Columns != rhs.Rows)
            {
                throw new MatrixException("Matrices are not compatible for multiplication");
            }
            Matrix matrix = new Matrix(lhs.Rows, rhs.Columns);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    double num3 = 0.0;
                    for (int k = 0; k < lhs.Columns; k++)
                    {
                        num3 += lhs[i, k] * rhs[k, j];
                    }
                    matrix[i, j] = num3;
                }
            }
            return matrix;
        }

        public static Matrix operator *(Matrix mat, double scalar)
        {
            Matrix matrix = new Matrix(mat.Rows, mat.Columns);
            for (var i = 0; i < mat.Rows; i++)
            {
                for (var j = 0; j < mat.Columns; j++)
                {
                    matrix[i, j] = mat[i, j] * scalar;
                }
            }
            return matrix;
        }
        public static Matrix Multiply(Matrix mat, double scalar)
        {
            Matrix matrix = new Matrix(mat.Rows, mat.Columns);
            for (var i = 0; i < mat.Rows; i++)
            {
                for (var j = 0; j < mat.Columns; j++)
                {
                    matrix[i, j] = mat[i, j] * scalar;
                }
            }
            return matrix;
        }
        public static Matrix operator *(double scalar, Matrix mat)
        {
            return (Matrix) (mat * scalar);
        }
        public static Matrix Multiply(double scalar, Matrix mat)
        {
            return (Matrix)(mat * scalar);
        }
        public static Matrix operator -(Matrix lhs, Matrix rhs)
        {
            if ((lhs.Rows != rhs.Rows) || (lhs.Columns != rhs.Columns))
            {
                throw new MatrixException("Matrices are not the same size");
            }
            Matrix matrix = new Matrix(lhs.Rows, lhs.Columns);
            for (var i = 0; i < lhs.Rows; i++)
            {
                for (var j = 0; j < lhs.Columns; j++)
                {
                    matrix[i, j] = lhs[i, j] - rhs[i, j];
                }
            }
            return matrix;
        }
        public static Matrix Subtract(Matrix lhs, Matrix rhs)
        {
            if ((lhs.Rows != rhs.Rows) || (lhs.Columns != rhs.Columns))
            {
                throw new MatrixException("Matrices are not the same size");
            }
            Matrix matrix = new Matrix(lhs.Rows, lhs.Columns);
            for (var i = 0; i < lhs.Rows; i++)
            {
                for (var j = 0; j < lhs.Columns; j++)
                {
                    matrix[i, j] = lhs[i, j] - rhs[i, j];
                }
            }
            return matrix;
        }
        public static Matrix operator -(Matrix mat, double scalar)
        {
            Matrix matrix = new Matrix(mat);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    Matrix matrix2;
                    int num3;
                    int num4;
                    (matrix2 = matrix)[num3 = i, num4 = j] = matrix2[num3, num4] - scalar;
                }
            }
            return matrix;
        }
        public static Matrix Subtract(Matrix mat, double scalar)
        {
            Matrix matrix = new Matrix(mat);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    Matrix matrix2;
                    int num3;
                    int num4;
                    (matrix2 = matrix)[num3 = i, num4 = j] = matrix2[num3, num4] - scalar;
                }
            }
            return matrix;
        }

        public static Matrix operator -(Matrix mat)
        {
            Matrix matrix = new Matrix(mat);
            for (var i = 0; i < mat.Rows; i++)
            {
                for (var j = 0; j < mat.Columns; j++)
                {
                    matrix[i, j] = -mat[i, j];
                }
            }
            return matrix;
        }
        public static Matrix Negate(Matrix mat)
        {
            Matrix matrix = new Matrix(mat);
            for (var i = 0; i < mat.Rows; i++)
            {
                for (var j = 0; j < mat.Columns; j++)
                {
                    matrix[i, j] = -mat[i, j];
                }
            }
            return matrix;
        }
        public Matrix PseudoInverse()
        {
            ISTUK.MathLibrary.SVD svd = new ISTUK.MathLibrary.SVD(this);
            Matrix matrix = new SquareMatrix(Columns);
            for (var i = 0; i < svd.W.Rows; i++)
            {
                if (svd.W[i] > 1E-05)
                {
                    matrix[i, i] = 1.0 / svd.W[i];
                }
            }
            return ((svd.V * matrix) * svd.U.Transpose());
        }

        public ISTUK.MathLibrary.QRDecomposition QRDecomposition()
        {
            return new ISTUK.MathLibrary.QRDecomposition(this);
        }

        public int Rank()
        {
            Matrix matrix = new Matrix(this);
            matrix.MakeRowEchelon();
            int num = 0;
            for (var i = 0; i < matrix.Rows; i++)
            {
                if (!matrix.IsRowZero(i))
                {
                    num++;
                }
            }
            return num;
        }

        public void SetColumn(int column, Vector vec)
        {
            for (var i = 0; i < Rows; i++)
            {
                this[i, column] = vec[i];
            }
        }

        public void SetRow(int row, params double[] vals)
        {
            for (var i = 0; i < Columns; i++)
            {
                this[row, i] = vals[i];
            }
        }

        public void SetRow(int row, Vector vec)
        {
            for (var i = 0; i < Columns; i++)
            {
                this[row, i] = vec[i];
            }
        }

        protected void SetSize(int _rows, int _columns)
        {
            rows = _rows;
            columns = _columns;
//            throw new NotImplementedException();
           elements = new double[_rows,_columns];
        }

        public Matrix SubMatrix(int firstRow, int lastRow, int firstColumn, int lastColumn)
        {
            Matrix matrix = new Matrix((lastRow - firstRow) + 1, (lastColumn - firstColumn) + 1);
            for (var i = firstRow; i <= lastRow; i++)
            {
                for (var j = firstColumn; j <= lastColumn; j++)
                {
                    matrix[i - firstRow, j - firstColumn] = this[i, j];
                }
            }
            return matrix;
        }

        public ISTUK.MathLibrary.SVD SVD()
        {
            return new ISTUK.MathLibrary.SVD(this);
        }

        public void SwapColumns(int column1, int column2)
        {
            Vector column = GetColumn(column1);
            Vector vec = GetColumn(column2);
            SetColumn(column1, vec);
            SetColumn(column2, column);
        }

        public void SwapRows(int row1, int row2)
        {
            Vector row = GetRow(row1);
            Vector vec = GetRow(row2);
            SetRow(row1, vec);
            SetRow(row2, row);
        }

        public override string ToString()
        {
          //  return ToString();
            return ToString(null, null);
        }

        public virtual string ToString(string format)
        {
            

            return ToString(format,null);
        }
        private string Splitmatrix()
        {
            var sb = new System.Text.StringBuilder();
            string start = ToString(); 



            return sb.ToString();
        }
    public virtual string ToString(string format, IFormatProvider formatProvider)
    {
        var sb = new System.Text.StringBuilder();
        if (format == null)
            format = "F2";
 
        if (format.ToUpper().StartsWith("F"))
        {
            //string[] t= System.Text.RegularExpressions.Regex.Split(ToString(),@"]");
            for (var r = 0; r < Rows; r++)
            {
                sb.Append("[");
                for (var c = 0; c < Columns; c++)
                {
                    sb.Append(this[r, c].ToString(format));
                    if ((c + 1) < Columns)
                        sb.Append(", ");
                }
                sb.Append("]");
                if ((r + 1) < Rows)
                    sb.Append(Environment.NewLine);
 
            }
            return sb.ToString();
        }
 
         if (!format.ToUpper(CultureInfo.InvariantCulture).StartsWith("MATLAB"))
             throw new FormatException("Invalid Format Specifier");
 
         sb = new System.Text.StringBuilder();
         sb.Append("[");
         for (var i = 0; i < Rows; i++)
         {
             for (int m = 0; m < Columns; m++)
             {
                 sb.Append(this[i, m].ToString("F5"));
                 if ((m + 1) < Columns)
                     sb.Append(", ");
             }
 
             if ((i + 1) < Rows)
                 sb.Append(";" + Environment.NewLine);
         }
         
         return (sb.ToString() + "]");
     }

    public Matrix Transpose()
    {
        var matrix = new Matrix(Columns, Rows);
        for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    matrix[j, i] = this[i, j];
                }
            }
            return matrix;
        }

        public int Columns
        {
            get
            {
                return columns;
            }
            set
            {
                SetSize(Rows, value);
            }
        }

        public double this[int row, int column] 
            {

        get
            {
                return elements[row,column];
            }
            set
            {
                elements[row,column] = value;
            }
        }

        public int Rows
        {
            get
            {
                return rows;
            }
            set
            {
                SetSize(value, Columns);
            }
        }

       
    }
    public enum MatrixDirection
    {
        RowWise,
        ColumnWise
    }
}

