namespace ISTUK.MathLibrary
{
    using System;
    using System.Collections.ObjectModel;

    public sealed class Plane3D : IGeometricElement3D
    {

        public Plane3D()
        {
            Point = new Point3D();
            Normal = new Vector3D();
        }

        public Plane3D(Point3D point, Vector3D normal)
        {
            Point = point;
            Normal = normal;
        }

        public Plane3D(Point3D p1, Point3D p2, Point3D p3)
        {
            var fitd = new LeastSquaresFit3D();
            var points = new Collection<Point3D> { p1, p2, p3};
            Point = fitd.Centroid(points);
            var vectord = (Vector3D) (p2 - p1);
            var vectord2 = (Vector3D) (p3 - p1);
            Normal = new Vector3D(Vector3D.Cross(vectord, vectord2).Normalised());
        }

        public Plane3D(double a, double b, double c)
        {
            Normal = new Vector3D(a, b, c);
            Normal.Normalise();
        }

        public static Plane3D FitToPoints(Collection<Point3D> points)
        {
            var fitd = new LeastSquaresFit3D();
            return fitd.FitPlaneToPoints(points);
        }

        public override string ToString()
        {
            return string.Format("Plane: Origin={0}, Normal={1}", Point, Normal);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("Plane: Origin={0}, Normal={1}", Point.ToString(format, formatProvider), Normal.ToString(format, formatProvider));
        }

        public double A
        {
            get
            {
                return Normal.X;
            }
        }

        public double B
        {
            get
            {
                return Normal.Y;
            }
        }

        public double C
        {
            get
            {
                return Normal.Z;
            }
        }

        public double D
        {
            get
            {
                return -Vector.Dot(Normal, (Vector3D) Point);
            }
        }

        TransformationMatrix3D IGeometricElement3D.Position
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Vector3D Normal { get; private set; }

        public Point3D Point { get; private set; }
    }
}

