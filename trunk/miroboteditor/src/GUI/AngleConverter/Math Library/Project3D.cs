namespace ISTUK.MathLibrary
{
    public static class Project3D
    {
        public static Point3D PointOntoCircle(Circle3D circle, Point3D point)
        {
            Plane3D plane = new Plane3D(circle.Origin, circle.Normal);
            Point3D pointd = PointOntoPlane(plane, point);
            Distance3D.Between(point, pointd);
            Vector3D vectord = (Vector3D) (circle.Origin - pointd);
            vectord.Normalise();
            return new Point3D();
//            return (circle.Origin + ((Point3D) (vectord * circle.Radius)));
        }

        public static Point3D PointOntoLine(Line3D line, Point3D point)
        {
            Vector3D vectord = (Vector3D) (line.Origin - point);
            if (vectord.Length() == 0.0)
            {
                return new Point3D(point);
            }
            double num = Vector.Dot(vectord, line.Direction);
            return new Point3D();
            //                return (line.Origin + ((Point3D) (line.Direction * num)));
        }

        public static Point3D PointOntoPlane(Plane3D plane, Point3D point)
        {
            Vector3D vectord = (Vector3D) (plane.Point - point);
            Vector3D vectord3 = Vector3D.Cross(Vector3D.Cross(vectord, plane.Normal), plane.Normal).Normalised();
            return new Point3D();
            //                return (plane.Point + ((Point3D) (vectord3 * Vector.Dot(vectord, vectord3))));
        }

        public static Point3D PointOntoSphere(Sphere3D sphere, Point3D point)
        {
            Vector3D vectord = (Vector3D) (point - sphere.Origin);
            
            vectord.Normalise();
            return new Point3D();
            //                return (sphere.Origin + ((Point3D) (vectord * sphere.Radius)));
        }
    }
}

