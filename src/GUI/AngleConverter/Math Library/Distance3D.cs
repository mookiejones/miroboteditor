namespace ISTUK.MathLibrary
{
    using System;

    public static class Distance3D
    {
        enum TYP3D { Point3d, Line3d, Plane3d, Circle3d, Sphere3d,None }
        private static TYP3D getType(IGeometricElement3D geo)
        {
            if (geo is Point3D)
                return TYP3D.Point3d;
            if(geo is Line3D)
                return TYP3D.Line3d;
            if(geo is Plane3D)
                return TYP3D.Plane3d;
            if (geo is Circle3D)
                return TYP3D.Circle3d;
            if (geo is Sphere3D)
                return TYP3D.Sphere3d;


            return TYP3D.None;
        }
        public static double Between(IGeometricElement3D e1, IGeometricElement3D e2)
        {
            if (e1 is Point3D)
            {
                Point3D E1 = e1 as Point3D;
                switch (getType(e2))
                {
                    case TYP3D.Point3d:
                        return PointToPoint(E1, e2 as Point3D);
                    case TYP3D.Line3d:
                        return PointToLine(e2 as Line3D, E1);
                    case TYP3D.Plane3d:
                        return PointToPlane(e2 as Plane3D, E1);
                    case TYP3D.Circle3d:
                        return PointToCircle(e2 as Circle3D, E1);
                    case TYP3D.Sphere3d:
                        return PointToSphere(e2 as Sphere3D, E1);
                }

            }
            else if (e1 is Line3D)
            {
                Line3D E1 = e1 as Line3D;
                switch (getType(e2))
                {
                    case TYP3D.Point3d:
                        return PointToLine(E1, e2 as Point3D);
                    case TYP3D.Line3d:
                        return LineToLine(E1, e2 as Line3D);
                    case TYP3D.Plane3d:
                    case TYP3D.Sphere3d:
                        throw new NotImplementedException();
                    case TYP3D.Circle3d:
                        return LineToCircle(e2 as Circle3D, E1);
                }

            }
            else if (e1 is Plane3D)
            {
                Plane3D E1 = e1 as Plane3D;
                switch (getType(e2))
                {
                    case TYP3D.Point3d:
                        return PointToPlane(E1, e2 as Point3D);
                    case TYP3D.Line3d:
                    case TYP3D.Plane3d:
                    case TYP3D.Circle3d:
                    case TYP3D.Sphere3d:
                        throw new NotImplementedException();
                }

            }
            else if (e1 is Circle3D)
            {
                Circle3D E1 = e1 as Circle3D;
                switch (getType(e2))
                {
                    case TYP3D.Point3d:
                        return PointToCircle(E1, e2 as Point3D);
                    case TYP3D.Line3d:
                    case TYP3D.Plane3d:
                    case TYP3D.Circle3d:
                    case TYP3D.Sphere3d:
                        throw new NotImplementedException();
                }
            }
            else if (e1 is Sphere3D)
            {
                Sphere3D E1 = e1 as Sphere3D;
                switch (getType(e2))
                {
                    case TYP3D.Point3d:
                        return PointToSphere(E1, e2 as Point3D);
                    case TYP3D.Line3d:
                    case TYP3D.Plane3d:
                    case TYP3D.Circle3d:
                    case TYP3D.Sphere3d:
                        throw new NotImplementedException();
                }
                throw new NotImplementedException();
                
            }
            return -1;
        }
        public static double CircleToCircle(Circle3D circle, Circle3D point)
        {
            throw new NotImplementedException();
        }

        public static double LineToCircle(Circle3D circle, Line3D point)
        {
            throw new NotImplementedException();
        }
       
        public static double LineToLine(Line3D line1, Line3D line2)
        {
            Point3D pointd;
            var pointd2 = new Point3D();
            return LineToLine(line1, line2, out pointd, out pointd2);
        }

        public static double LineToLine(Line3D line1, Line3D line2, out Point3D closestPoint1, out Point3D closestPoint2)
        {
            double num7;
            double num8;
            double num9;
            Point3D origin = line1.Origin;
            Point3D pointd2 = line2.Origin;
            Vector3D direction = line1.Direction;
            Vector3D vectord2 = line2.Direction;
            var vectord3 = (Vector3D) (origin - pointd2);
            double num = Vector.Dot(-direction, vectord2);
            double num2 = Vector.Dot(vectord3, direction);
            double num3 = vectord3.Length() * vectord3.Length();
            double num4 = Math.Abs((double) (1.0 - (num * num)));
            if (num4 > 1E-05)
            {
                double num5 = Vector.Dot(-vectord3, vectord2);
                double num6 = 1.0 / num4;
                num7 = ((num * num5) - num2) * num6;
                num8 = ((num * num2) - num5) * num6;
                num9 = ((num7 * ((num7 + (num * num8)) + (2.0 * num2))) + (num8 * (((num * num7) + num8) + (2.0 * num5)))) + num3;
            }
            else
            {
                num7 = -num2;
                num8 = 0.0;
                num9 = (num2 * num7) + num3;
            }
            closestPoint1 = origin + new Vector3D((Matrix) (num7 * direction));
            closestPoint2 = pointd2 + new Vector3D((Matrix) (num8 * vectord2));
            return Math.Sqrt(num9);
        }

        public static double PlaneToCircle(Circle3D circle, Plane3D point)
        {
            throw new NotImplementedException();
        }

        public static double PointToCircle(Circle3D circle, Point3D point)
        {
            Plane3D plane = new Plane3D(circle.Origin, circle.Normal);
            Point3D pointd = Project3D.PointOntoPlane(plane, point);
            Between(point, pointd);
            Vector3D vectord = (Vector3D) (circle.Origin - pointd);
            vectord.Normalise();
            //Point3D pointd2 = circle.Origin + ((Point3D) (vectord * circle.Radius));
            Point3D pointd2 = new Point3D();
            return PointToPoint(point, pointd2);
        }

        public static double PointToLine(Line3D line, Point3D point)
        {
            return Vector3D.Cross(line.Direction, (Vector) (line.Origin - point)).Length();
        }

        public static double PointToPlane(Plane3D plane, Point3D point)
        {
            return Vector.Dot(plane.Normal, (Vector) (point - plane.Point));
        }

        public static double PointToPoint(Point3D p1, Point3D p2)
        {
            return ((p1 - p2)).Length();
        }

        public static double PointToSphere(Sphere3D sphere, Point3D point)
        {
            return (((point - sphere.Origin)).Length() - sphere.Radius);
        }
    }
}

