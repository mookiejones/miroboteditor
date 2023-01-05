using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using miRobotEditor.Controls.AngleConverter;
using miRobotEditor.Controls.AngleConverter.Classes;
using miRobotEditor.Controls.AngleConverter.Exceptions;
using miRobotEditor.Controls.AngleConverter.Interfaces;

namespace miRobotEditor.Classes
{
    [Localizable(false)]
    public sealed class LeastSquaresFit3D
    {
        private Collection<Point3D> _measuredPoints;
        private NRSolver _solver;
        private double AverageError { get; set; }
        private Collection<double> Errors { get; set; }
        private double MaxError { get; set; }
        private int PointWithLargestError { get; set; }

        private double RmsError
        {
            get
            {
                double num = Errors.Sum((double num2) => num2 * num2);
                num /= Errors.Count;
                return Math.Sqrt(num);
            }
        }

        private double StandardDeviationError { get; set; }
        private TransformationMatrix3D Transform { get; set; }

        private void CalculateErrors(IList<Point3D> points, IGeometricElement3D element)
        {
            Errors = new Collection<double>();
            double num = 0.0;
            double num2 = 0.0;
            MaxError = 0.0;
            PointWithLargestError = -1;
            for (int i = 0; i < points.Count; i++)
            {
                Point3D e = points[i];
                double num3 = Distance3D.Between(element, e);
                Errors.Add(num3);
                num += num3;
                num2 += num3 * num3;
                if (num3 > MaxError)
                {
                    MaxError = num3;
                    PointWithLargestError = i;
                }
            }
            int count = points.Count;
            AverageError = num / count;
            StandardDeviationError = Math.Sqrt((count * num2) - (AverageError * AverageError)) / count;
        }

        public Point3D Centroid(Collection<Point3D> points)
        {
            int count = points.Count;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            foreach (Point3D current in points)
            {
                num += current.X;
                num2 += current.Y;
                num3 += current.Z;
            }
            Point3D point3D = new Point3D(num / count, num2 / count, num3 / count);
            Transform = new TransformationMatrix3D(new Vector3D(point3D.X, point3D.Y, point3D.Z), new RotationMatrix3D());
            CalculateErrors(points, point3D);
            return point3D;
        }

        private Vector Circle3DErrorFunction(Vector vec)
        {
            Vector vector = new Vector(_solver.NumEquations);
            int index = 0;
            Point3D point3D = new Point3D(vec[0], vec[1], vec[2]);
            Plane3D plane = new Plane3D(point3D, new Vector3D(vec[3], vec[4], vec[5]));
            foreach (Point3D current in _measuredPoints)
            {
                Point3D p = Project3D.PointOntoPlane(plane, current);
                Vector3D vector3D = point3D - p;
                vector3D.Normalise();
                Point3D point3D2 = new Point3D();
                vector[index++] = current.X - point3D2.X;
                vector[index++] = current.Y - point3D2.Y;
                vector[index++] = current.Z - point3D2.Z;
            }
            vector[index] = new Vector3D(vec[3], vec[4], vec[5]).Length() - 1.0;
            return vector;
        }

        public Circle3D FitCircleToPoints(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new MatrixNullReference();
            }
            if (points.Count < 3)
            {
                throw new ArgumentException("Need at least 3 points to fit circle");
            }
            _solver = new NRSolver((points.Count * 3) + 1, 7);
            _measuredPoints = points;
            LeastSquaresFit3D leastSquaresFit3D = new LeastSquaresFit3D();
            Plane3D plane3D = leastSquaresFit3D.FitPlaneToPoints(points);
            Circle3D initialGuess = new Circle3D
            {
                Origin = leastSquaresFit3D.Centroid(points),
                Normal = plane3D.Normal,
                Radius = leastSquaresFit3D.AverageError
            };
            Vector vector = _solver.Solve(Circle3DErrorFunction, VectorFromCircle3D(initialGuess));
            Circle3D circle3D = new Circle3D
            {
                Origin = new Point3D(vector[0], vector[1], vector[2]),
                Normal = new Vector3D(vector[3], vector[4], vector[5]),
                Radius = vector[6]
            };
            CalculateErrors(points, circle3D);
            return circle3D;
        }

        public Circle3D FitCircleToPoints2(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new MatrixNullReference();
            }
            if (points.Count < 3)
            {
                throw new ArgumentException("Need at least 3 points to fit circle");
            }
            Circle3D circle3D = new Circle3D();
            LeastSquaresFit3D leastSquaresFit3D = new LeastSquaresFit3D();
            Matrix matrix = new Matrix(points.Count, 7);
            Vector vector = new Vector(points.Count);
            for (int i = 0; i < 50; i++)
            {
                circle3D.Origin = leastSquaresFit3D.Centroid(points);
                circle3D.Radius = leastSquaresFit3D.RmsError;
                Plane3D plane3D = leastSquaresFit3D.FitPlaneToPoints(points);
                circle3D.Normal = plane3D.Normal;
                int num = 0;
                foreach (Point3D current in points)
                {
                    Point3D p = Project3D.PointOntoPlane(plane3D, current);
                    Vector3D vector3D = circle3D.Origin - p;
                    vector3D.Normalise();
                    matrix[num, 0] = vector3D[0];
                    matrix[num, 1] = vector3D[1];
                    matrix[num, 2] = vector3D[2];
                    matrix[num, 3] = plane3D.Normal.X;
                    matrix[num, 4] = plane3D.Normal.Y;
                    matrix[num, 5] = plane3D.Normal.Z;
                    matrix[num, 6] = -1.0;
                    double value = Distance3D.PointToCircle(circle3D, current);
                    vector[num] = value;
                    num++;
                }
                Vector vector2 = matrix.PseudoInverse() * vector;
                if (vector2.Length() < 1E-06)
                {
                    break;
                }
                Point3D origin = circle3D.Origin;
                origin.X += vector2[0];
                Point3D origin2 = circle3D.Origin;
                origin2.Y += vector2[1];
                Point3D origin3 = circle3D.Origin;
                origin3.Z += vector2[2];
                Vector3D normal = circle3D.Normal;
                normal.X += vector2[3];
                Vector3D normal2 = circle3D.Normal;
                normal2.Y += vector2[4];
                Vector3D normal3 = circle3D.Normal;
                normal3.Z += vector2[5];
                circle3D.Radius -= vector2[6];
            }
            CalculateErrors(points, circle3D);
            return circle3D;
        }

        public Line3D FitLineToPoints(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new MatrixNullReference();
            }
            Point3D point3D = Centroid(points);
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            foreach (Point3D current in points)
            {
                double num7 = current.X - point3D.X;
                double num8 = current.Y - point3D.Y;
                double num9 = current.Z - point3D.Z;
                num += num7 * num7;
                num2 += num8 * num8;
                num3 += num9 * num9;
                num4 += num7 * num8;
                num5 += num8 * num9;
                num6 += num7 * num9;
            }
            SquareMatrix mat = new SquareMatrix(3, new[]
            {
                num2 + num3,
                -num4,
                -num6,
                -num4,
                num3 + num,
                -num5,
                -num6,
                -num5,
                num + num2
            });
            SVD sVD = new SVD(mat);
            Vector3D direction = new Vector3D(sVD.U.GetColumn(sVD.SmallestSingularIndex));
            Line3D line3D = new Line3D(point3D, direction);
            CalculateErrors(points, line3D);
            return line3D;
        }

        public Plane3D FitPlaneToPoints(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new NullReferenceException();
            }
            if (points.Count < 3)
            {
                throw new MatrixException("Not enough points to fit a plane");
            }
            Point3D point3D = Centroid(points);
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            foreach (Point3D current in points)
            {
                double num7 = current.X - point3D.X;
                double num8 = current.Y - point3D.Y;
                double num9 = current.Z - point3D.Z;
                num += num7 * num7;
                num2 += num8 * num8;
                num3 += num9 * num9;
                num4 += num7 * num8;
                num5 += num8 * num9;
                num6 += num7 * num9;
            }
            SquareMatrix mat = new SquareMatrix(3, new[]
            {
                num,
                num4,
                num6,
                num4,
                num2,
                num5,
                num6,
                num5,
                num3
            });
            SVD sVD = new SVD(mat);
            Vector3D normal = new Vector3D(sVD.U.GetColumn(sVD.SmallestSingularIndex));
            Plane3D plane3D = new Plane3D(point3D, normal);
            CalculateErrors(points, plane3D);
            return plane3D;
        }

        public Sphere3D FitSphereToPoints(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new NullReferenceException();
            }
            if (points.Count < 4)
            {
                throw new MatrixException("Need at least 4 points to fit sphere");
            }
            Sphere3D sphere3D = new Sphere3D();
            LeastSquaresFit3D leastSquaresFit3D = new LeastSquaresFit3D();
            sphere3D.Origin = leastSquaresFit3D.Centroid(points);
            sphere3D.Radius = leastSquaresFit3D.RmsError;
            Matrix matrix = new Matrix(points.Count, 4);
            Vector vector = new Vector(points.Count);
            for (int i = 0; i < 50; i++)
            {
                int num = 0;
                foreach (Point3D current in points)
                {
                    Vector3D vector3D = Project3D.PointOntoSphere(sphere3D, current) - sphere3D.Origin;
                    vector3D.Normalise();
                    matrix[num, 0] = vector3D.X;
                    matrix[num, 1] = vector3D.Y;
                    matrix[num, 2] = vector3D.Z;
                    matrix[num, 3] = -1.0;
                    double value = Distance3D.PointToSphere(sphere3D, current);
                    vector[num] = value;
                    num++;
                }
                Vector vector2 = matrix.PseudoInverse() * vector;
                if (vector2.Length() < 1E-06)
                {
                    break;
                }
                Point3D origin = sphere3D.Origin;
                origin.X += vector2[0];
                Point3D origin2 = sphere3D.Origin;
                origin2.Y += vector2[1];
                Point3D origin3 = sphere3D.Origin;
                origin3.Z += vector2[2];
                sphere3D.Radius -= vector2[3];
            }
            CalculateErrors(points, sphere3D);
            return sphere3D;
        }

        private static Vector VectorFromCircle3D(Circle3D initialGuess)
        {
            Vector vector = new Vector(7);
            vector[0] = initialGuess.Origin.X;
            vector[1] = initialGuess.Origin.Y;
            vector[2] = initialGuess.Origin.Z;
            vector[3] = initialGuess.Normal.X;
            vector[4] = initialGuess.Normal.Y;
            vector[5] = initialGuess.Normal.Z;
            vector[6] = initialGuess.Radius;
            return vector;
        }
    }

    public sealed class NRSolver
    {
        private const int MaxIterations = 20;
        private const double StopCondition = 1E-07;

        public NRSolver(int numEquations, int numVariables)
        {
            NumEquations = numEquations;
            NumVariables = numVariables;
        }

        public int NumEquations { get; private set; }
        private int NumVariables { get; set; }
        private int NumStepsToConverge { get; set; }

        private Matrix CalculateJacobian(ErrorFunction errorFunction, Vector guess)
        {
            Matrix matrix = new Matrix(NumEquations, NumVariables);
            for (int i = 0; i < matrix.Columns; i++)
            {
                double num = (Math.Abs(guess[i]) >= 1.0) ? (Math.Abs(guess[i]) * 1E-07) : 1E-07;
                Vector vector = new Vector(guess);
                Vector vector2;
                int index;
                (vector2 = vector)[index = i] = vector2[index] + num;
                Vector v = errorFunction(vector);
                Vector vector3;
                int index2;
                (vector3 = vector)[index2 = i] = vector3[index2] - (2.0 * num);
                Vector v2 = errorFunction(vector);
                Vector vec = v - v2;
                matrix.SetColumn(i, vec / (2.0 * num));
            }
            return matrix;
        }

        private static bool IsDone(Vector delta)
        {
            bool result;
            for (int i = 0; i < delta.Rows; i++)
            {
                if (Math.Abs(delta[i]) > 1E-07)
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }

        public Vector Solve(ErrorFunction errorFunction, Vector initialGuess)
        {
            if (initialGuess.Size != NumVariables)
            {
                throw new MatrixException("Size of the initial guess vector is not correct");
            }
            Vector vector = new Vector(initialGuess);
            NumStepsToConverge = 0;
            Vector result;
            for (int i = 0; i < 20; i++)
            {
                Matrix matrix = CalculateJacobian(errorFunction, vector);
                Vector vec = errorFunction(vector);
                Matrix matrix2 = matrix.Transpose();
                SquareMatrix squareMatrix = new SquareMatrix(matrix2 * matrix);
                Vector vec2 = matrix2 * vec;
                Vector vector2 = squareMatrix.PseudoInverse() * vec2;
                vector -= vector2;
                if (IsDone(vector2))
                {
                    NumStepsToConverge = i + 1;
                    result = vector;
                    return result;
                }
            }
            result = vector;
            return result;
        }
    }

    public delegate Vector ErrorFunction(Vector vec);

    public static class Distance3D
    {
        private static TYP3D getType(IGeometricElement3D geo)
        {
            TYP3D result;
            if (geo is Point3D)
            {
                result = TYP3D.Point3D;
            }
            else
            {
                result = geo is Line3D
                    ? TYP3D.Line3D
                    : geo is Plane3D ? TYP3D.Plane3D : geo is Circle3D ? TYP3D.Circle3D : geo is Sphere3D ? TYP3D.Sphere3D : TYP3D.None;
            }
            return result;
        }

        public static double Between(IGeometricElement3D e1, IGeometricElement3D e2)
        {
            double result;
            if (e1 is Point3D)
            {
                Point3D point3D = e1 as Point3D;
                switch (getType(e2))
                {
                    case TYP3D.Point3D:
                        result = PointToPoint(point3D, e2 as Point3D);
                        return result;

                    case TYP3D.Line3D:
                        result = PointToLine(e2 as Line3D, point3D);
                        return result;

                    case TYP3D.Plane3D:
                        result = PointToPlane(e2 as Plane3D, point3D);
                        return result;

                    case TYP3D.Circle3D:
                        result = PointToCircle(e2 as Circle3D, point3D);
                        return result;

                    case TYP3D.Sphere3D:
                        result = PointToSphere(e2 as Sphere3D, point3D);
                        return result;
                }
            }
            else
            {
                if (e1 is Line3D)
                {
                    Line3D line3D = e1 as Line3D;
                    switch (getType(e2))
                    {
                        case TYP3D.Point3D:
                            result = PointToLine(line3D, e2 as Point3D);
                            return result;

                        case TYP3D.Line3D:
                            result = LineToLine(line3D, e2 as Line3D);
                            return result;

                        case TYP3D.Plane3D:
                        case TYP3D.Sphere3D:
                            throw new NotImplementedException();
                        case TYP3D.Circle3D:
                            result = LineToCircle(e2 as Circle3D, line3D);
                            return result;
                    }
                }
                else
                {
                    if (e1 is Plane3D)
                    {
                        Plane3D plane = e1 as Plane3D;
                        switch (getType(e2))
                        {
                            case TYP3D.Point3D:
                                result = PointToPlane(plane, e2 as Point3D);
                                return result;

                            case TYP3D.Line3D:
                            case TYP3D.Plane3D:
                            case TYP3D.Circle3D:
                            case TYP3D.Sphere3D:
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        if (e1 is Circle3D)
                        {
                            Circle3D circle = e1 as Circle3D;
                            switch (getType(e2))
                            {
                                case TYP3D.Point3D:
                                    result = PointToCircle(circle, e2 as Point3D);
                                    return result;

                                case TYP3D.Line3D:
                                case TYP3D.Plane3D:
                                case TYP3D.Circle3D:
                                case TYP3D.Sphere3D:
                                    throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            if (e1 is Sphere3D)
                            {
                                Sphere3D sphere = e1 as Sphere3D;
                                switch (getType(e2))
                                {
                                    case TYP3D.Point3D:
                                        result = PointToSphere(sphere, e2 as Point3D);
                                        return result;

                                    case TYP3D.Line3D:
                                    case TYP3D.Plane3D:
                                    case TYP3D.Circle3D:
                                    case TYP3D.Sphere3D:
                                        throw new NotImplementedException();
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                        }
                    }
                }
            }
            result = -1.0;
            return result;
        }

        private static double LineToCircle(Circle3D circle, Line3D point)
        {
            throw new NotImplementedException();
        }

        private static double LineToLine(Line3D line1, Line3D line2)
        {
            return LineToLine(line1, line2, out _, out _);
        }

        private static double LineToLine(Line3D line1, Line3D line2, out Point3D closestPoint1,
            out Point3D closestPoint2)
        {
            Point3D origin = line1.Origin;
            Point3D origin2 = line2.Origin;
            Vector3D direction = line1.Direction;
            Vector3D direction2 = line2.Direction;
            Vector3D vector3D = origin - origin2;
            double num = Vector.Dot(-direction, direction2);
            double num2 = Vector.Dot(vector3D, direction);
            double num3 = vector3D.Length() * vector3D.Length();
            double num4 = Math.Abs(1.0 - (num * num));
            double num7;
            double num8;
            double d;
            if (num4 > 1E-05)
            {
                double num5 = Vector.Dot(-vector3D, direction2);
                double num6 = 1.0 / num4;
                num7 = ((num * num5) - num2) * num6;
                num8 = ((num * num2) - num5) * num6;
                d = (num7 * (num7 + (num * num8) + (2.0 * num2))) + (num8 * ((num * num7) + num8 + (2.0 * num5))) + num3;
            }
            else
            {
                num7 = -num2;
                num8 = 0.0;
                d = (num2 * num7) + num3;
            }
            closestPoint1 = origin + new Vector3D(num7 * direction);
            closestPoint2 = origin2 + new Vector3D(num8 * direction2);
            return Math.Sqrt(d);
        }

        public static double PointToCircle(Circle3D circle, Point3D point)
        {
            Plane3D plane = new Plane3D(circle.Origin, circle.Normal);
            Point3D point3D = Project3D.PointOntoPlane(plane, point);
            _ = Between(point, point3D);
            Vector3D vector3D = circle.Origin - point3D;
            vector3D.Normalise();
            Point3D p = new Point3D();
            return PointToPoint(point, p);
        }

        private static double PointToLine(Line3D line, Point3D point)
        {
            return Vector3D.Cross(line.Direction, line.Origin - point).Length();
        }

        private static double PointToPlane(Plane3D plane, Point3D point)
        {
            return Vector.Dot(plane.Normal, point - plane.Point);
        }

        private static double PointToPoint(Point3D p1, Point3D p2)
        {
            return (p1 - p2).Length();
        }

        public static double PointToSphere(Sphere3D sphere, Point3D point)
        {
            return (point - sphere.Origin).Length() - sphere.Radius;
        }

        private enum TYP3D
        {
            Point3D,
            Line3D,
            Plane3D,
            Circle3D,
            Sphere3D,
            None
        }
    }
}