using System;
using System.Collections.ObjectModel;

namespace AngleConverterControl.Model
{
    public sealed class Vector3D : Vector, IGeometricElement3D, IFormattable
    {
        public Vector3D()
            : base(3)
        {
        }

        public Vector3D(Matrix mat)
            : base(3)
        {
            if (mat.Rows != 1 && mat.Columns != 1)
            {
                throw new MatrixException("Cannot convert Matrix to Vector3D. It has more than one row or column");
            }
            if (mat.Columns == 3 && mat.Rows == 1)
            {
                mat = mat.Transpose();
            }
            for (int i = 0; i < 3; i++)
            {
                base[i] = mat[i, 0];
            }
        }

        public Vector3D(Vector vec)
            : base(3)
        {
            for (int i = 0; i < 3; i++)
            {
                base[i] = vec[i];
            }
        }

        public Vector3D(double x, double y, double z)
            : base(3, new[]
            {
                x,
                y,
                z
            })
        {
        }

        public double X
        {
            get => base[0];
            set => base[0] = value;
        }

        public double Y
        {
            get => base[1];
            set => base[1] = value;
        }

        public double Z
        {
            get => base[2];
            set => base[2] = value;
        }

        TransformationMatrix3D IGeometricElement3D.Position => throw new NotImplementedException();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static double Angle(Vector vec1, Vector vec2)
        {
            double num = Dot(vec1, vec2);
            double num2 = vec1.Length() * vec2.Length();
            return Math.Acos(num / num2) * 180.0 / 3.1415926535897931;
        }

        public static Vector3D Cross(Vector vec1, Vector vec2)
        {
            Vector3D vector3D = new Vector3D();
            vector3D[0] = (vec1[1] * vec2[2]) - (vec1[2] * vec2[1]);
            vector3D[1] = (vec1[2] * vec2[0]) - (vec1[0] * vec2[2]);
            vector3D[2] = (vec1[0] * vec2[1]) - (vec1[1] * vec2[0]);
            return vector3D;
        }

        public static Vector3D NaN()
        {
            return new Vector3D(NaN(3, 1));
        }

        public new Vector3D Normalised()
        {
            return new Vector3D(this / base.Length());
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1 + v2);
        }

        public static Vector3D Add(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1 + v2);
        }

        public static Vector3D operator /(Vector3D vec, double scalar)
        {
            return new Vector3D(vec / scalar);
        }

        public static Vector3D Divide(Vector3D vec, double scalar)
        {
            return new Vector3D(vec / scalar);
        }

        public static explicit operator Point3D(Vector3D vec)
        {
            return new Point3D(vec);
        }

        public static Point3D ToPoint3D(Vector3D vec)
        {
            return new Point3D(vec);
        }

        public static Vector3D operator *(Vector3D vec, double scalar)
        {
            return new Vector3D(vec * scalar);
        }

        public static Vector3D Multiply(Vector3D vec, double scalar)
        {
            return new Vector3D(vec * scalar);
        }

        public static Collection<Vector3D> operator *(Collection<TransformationMatrix3D> transforms, Vector3D vector)
        {
            Collection<Vector3D> collection = new Collection<Vector3D>();
            foreach (TransformationMatrix3D current in transforms)
            {
                collection.Add(current * vector);
            }
            return collection;
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1 - v2);
        }

        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            return v1 == v2;
        }

        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return !(v1 == v2);
        }

        public static Vector3D Subtract(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1 - v2);
        }

        public static Vector3D operator -(Vector3D vec)
        {
            return Negate(vec);
        }

        public static Vector3D Negate(Vector3D vec)
        {
            return new Vector3D(-vec.X, -vec.Y, -vec.Z);
        }
    }
}
