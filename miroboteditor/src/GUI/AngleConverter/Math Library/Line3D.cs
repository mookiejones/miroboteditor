namespace ISTUK.MathLibrary
{
    using System;
    using System.Collections.ObjectModel;

    public sealed class Line3D : IGeometricElement3D
    {
       

        public Line3D()
        {
            Origin = new Point3D();
            Direction = new Vector3D();
        }

        public Line3D(Point3D p1, Point3D p2)
        {
            Origin = p1;
            Direction = (Vector3D)(p2 - p1);
            Direction.Normalise();
        }

        public Line3D(Point3D origin, Vector3D direction)
        {
            Origin = origin;
            Direction = direction;
            Direction.Normalise();
        }

        public static Line3D FitToPoints(Collection<Point3D> points)
        {
            var fitd = new LeastSquaresFit3D();
            return fitd.FitLineToPoints(points);
        }

        public Point3D GetPoint(double u)
        {
            var vectord = new Vector3D((Matrix)(u * Direction));
            return (Origin + vectord);
        }

        public override string ToString()
        {
            return string.Format("Line: Origin={0}, Direction={1}", Origin, Direction);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("Line: Origin={0}, Direction={1}", Origin.ToString(format, formatProvider), Direction.ToString(format, formatProvider));
        }

        public Vector3D Direction { get; private set; }

        TransformationMatrix3D IGeometricElement3D.Position
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Point3D Origin { get; private set; }
       
    }
}

