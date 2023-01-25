using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using miRobotEditor.Controls.AngleConverter.Classes;
using miRobotEditor.Controls.AngleConverter.Interfaces;

namespace miRobotEditor.Classes
{
    [Localizable(false)]
    public sealed class Plane3D : IGeometricElement3D
    {
        public Plane3D(Point3D point, Vector3D normal)
        {
            Point = point;
            Normal = normal;
        }

        public double A
        {
            get { return Normal.X; }
        }

        public double B
        {
            get { return Normal.Y; }
        }

        public double C
        {
            get { return Normal.Z; }
        }

        public double D
        {
            get { return -Vector.Dot(Normal, (Vector3D) Point); }
        }

        public Vector3D Normal { get; private set; }
        public Point3D Point { get; private set; }

        TransformationMatrix3D IGeometricElement3D.Position
        {
            get { throw new NotImplementedException(); }
        }

        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            return string.Format("Plane: Origin={0}, Normal={1}", Point.ToString(format, formatProvider),
                Normal.ToString(format, formatProvider));
        }

        public static Plane3D FitToPoints(Collection<Point3D> points)
        {
            var leastSquaresFit3D = new LeastSquaresFit3D();
            return leastSquaresFit3D.FitPlaneToPoints(points);
        }

        public override string ToString()
        {
            return string.Format("Plane: Origin={0}, Normal={1}", Point, Normal);
        }
    }
}