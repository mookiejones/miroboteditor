using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace miRobotEditor.GUI.AngleConverter
{
    [Localizable(false)]
    public sealed class Plane3D : IGeometricElement3D
    {
        public Plane3D(Point3D point, Vector3D normal)
        {
            Point = point;
            Normal = normal;
        }

        public double A => Normal.X;

        public double B => Normal.Y;

        public double C => Normal.Z;

        public double D => -Vector.Dot(Normal, (Vector3D)Point);

        public Vector3D Normal { get; private set; }

        public Point3D Point { get; private set; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("Plane: Origin={0}, Normal={1}", Point.ToString(format, formatProvider),
                Normal.ToString(format, formatProvider));
        }

        TransformationMatrix3D IGeometricElement3D.Position => throw new NotImplementedException();

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
    }
}