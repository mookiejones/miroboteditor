namespace ISTUK.MathLibrary
{
    public class Vector2D : Vector
    {
        public Vector2D() : base(2)
        {
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public Vector2D(Matrix mat) : base(2)
        {
            if ((mat.Rows != 1) && (mat.Columns != 1))
            {
                throw new MatrixException("Cannot convert matrix to Vector3D. It has more than one row or column");
            }
            if ((mat.Rows == 1) && (mat.Columns == 2))
            {
                mat = mat.Transpose();
            }
            for (var i = 0; i < 2; i++)
            {
                base[i] = mat[i, 0];
            }
        }

        public Vector2D(Vector2D vec) : base(2)
        {
            base[0] = vec[0];
            base[1] = vec[1];
        }

        public Vector2D(double x, double y) : base(2, new double[] { x, y })
        {
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1 + v2);
        }
        public static Vector2D Add(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1 + v2);
        }
        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return (v1 == v2);
        }
        public static bool Equals(Vector2D v1, Vector2D v2)
        {
            return (v1 == v2);
        }
        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return !(v1 == v2);
        }
        public static Vector2D operator /(Vector2D vec, double scalar)
        {
            return new Vector2D((Matrix) (vec / scalar));
        }
        public static Vector2D Divide(Vector2D vec, double scalar)
        {
            return new Vector2D((Matrix)(vec / scalar));
        }
    }
}

