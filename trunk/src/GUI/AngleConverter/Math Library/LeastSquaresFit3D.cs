using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ISTUK.MathLibrary
{
    using System;
    using System.Collections.ObjectModel;

    [Localizable(false)]
    public class LeastSquaresFit3D
    {
        private double _averageError;
        private Collection<double> _errors;
        private double _maxError;
        private Collection<Point3D> _measuredPoints;
        private int _pointWithLargestError;
        private NRSolver _solver;
        private double _standardDeviationError;
        private TransformationMatrix3D _transform;

        public Vector3D AverageVector3D(Collection<Vector3D> vectors)
        {


            var count = vectors.Count;
            var vectord = new Vector3D();
            vectord = vectors.Aggregate(vectord, (current, vectord2) => current + vectord2);
            var element = vectord / count;
            CalculateErrors(vectors, element);
            return element;
        }

        private void CalculateErrors(Collection<Point3D> points, IGeometricElement3D element)
        {
            _errors = new Collection<double>();
            var num = 0.0;
            var num2 = 0.0;
            _maxError = 0.0;
            _pointWithLargestError = -1;
            for (var i = 0; i < points.Count; i++)
            {
                var pointd = points[i];
                var item = Distance3D.Between(element, pointd);
                _errors.Add(item);
                num += item;
                num2 += item * item;
                if (item > _maxError)
                {
                    _maxError = item;
                    _pointWithLargestError = i;
                }
            }
            var count = points.Count;
            _averageError = num / count;
            _standardDeviationError = Math.Sqrt((count * num2) - (_averageError * _averageError)) / count;
        }

        private void CalculateErrors(IEnumerable<Vector3D> vectors, IGeometricElement3D element)
        {
            var points = new Collection<Point3D>();
            foreach (var vectord in vectors)
            {
                points.Add(new Point3D(vectord.X, vectord.Y, vectord.Z));
            }
            CalculateErrors(points, element);
        }

        private void CalculateErrors(Collection<Point3D> nominal, Collection<Point3D> measured, TransformationMatrix3D transform)
        {
            _errors = new Collection<double>();
            var num = 0.0;
            var num2 = 0.0;
            _maxError = 0.0;
            _pointWithLargestError = -1;
            for (var i = 0; i < nominal.Count; i++)
            {
                var pointd = measured[i];
                var vectord = ((Vector3D) pointd) - transform.Translation;
                var pointd2 = new Point3D(new Vector3D(transform.Rotation.Inverse() * vectord));
                var pointd3 = nominal[i];
                var item = Distance3D.Between(pointd2, pointd3);
                _errors.Add(item);
                num += item;
                num2 += item * item;
                if (item > _maxError)
                {
                    _maxError = item;
                    _pointWithLargestError = i;
                }
            }
            var count = nominal.Count;
            _averageError = num / count;
            _standardDeviationError = Math.Sqrt((count * num2) - (_averageError * _averageError)) / count;
        }

        public Point3D Centroid(Collection<Point3D> points)
        {
            var count = points.Count;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0.0;
            foreach (var pointd in points)
            {
                num2 += pointd.X;
                num3 += pointd.Y;
                num4 += pointd.Z;
            }
            var element = new Point3D(num2 / count, num3 / count, num4 / count);
            _transform = new TransformationMatrix3D(new Vector3D(element.X, element.Y, element.Z), new RotationMatrix3D());
            CalculateErrors(points, element);
            return element;
        }

        private Vector Circle3DErrorFunction(Vector vec)
        {
            var vector = new Vector(_solver.NumEquations);
            var num = 0;
            var point = new Point3D(vec[0], vec[1], vec[2]);
            var plane = new Plane3D(point, new Vector3D(vec[3], vec[4], vec[5]));
            foreach (var pointd2 in _measuredPoints)
            {
                var pointd3 = Project3D.PointOntoPlane(plane, pointd2);
                var vectord = point - pointd3;
                vectord.Normalise();
                var pointd4 = new Point3D();
//                Point3D pointd4 = point + ((Point3D) (vectord * num2));
                vector[num++] = pointd2.X - pointd4.X;
                vector[num++] = pointd2.Y - pointd4.Y;
                vector[num++] = pointd2.Z - pointd4.Z;
            }
            vector[num] = new Vector3D(vec[3], vec[4], vec[5]).Length() - 1.0;
            return vector;
        }

        public Circle3D FitCircleToPoints(Collection<Point3D> points)
        {
            if (points == null) throw new MatrixNullReference();
            if (points.Count < 3)
            {
                throw new ArgumentException("Need at least 3 points to fit circle");
            }
            _solver = new NRSolver((points.Count * 3) + 1, 7);
            _measuredPoints = points;
            var fitd = new LeastSquaresFit3D();
            var planed = fitd.FitPlaneToPoints(points);
            var initialGuess = new Circle3D {
                Origin = fitd.Centroid(points),
                Normal = planed.Normal,
                Radius = fitd.AverageError,
//                Radius = fitd.RMSError
            };
            var vector = _solver.Solve(Circle3DErrorFunction, VectorFromCircle3D(initialGuess));
            var element = new Circle3D {
                Origin = new Point3D(vector[0], vector[1], vector[2]),
                Normal = new Vector3D(vector[3], vector[4], vector[5]),
                Radius = vector[6]
            };
            CalculateErrors(points, element);
            return element;
        }

        public Circle3D FitCircleToPoints2(Collection<Point3D> points)
        {
            if (points == null) throw new MatrixNullReference();

            if (points.Count < 3)
            {
                throw new ArgumentException("Need at least 3 points to fit circle");
            }
            var circle = new Circle3D();
            var fitd = new LeastSquaresFit3D();
            var matrix = new Matrix(points.Count, 7);
            var vector = new Vector(points.Count);
            for (var i = 0; i < 50; i++)
            {
                circle.Origin = fitd.Centroid(points);
                circle.Radius = fitd.RmsError;
                var plane = fitd.FitPlaneToPoints(points);
                circle.Normal = plane.Normal;
                var num2 = 0;
                foreach (var pointd in points)
                {
                    var pointd2 = Project3D.PointOntoPlane(plane, pointd);
                    var vectord = circle.Origin - pointd2;
                    vectord.Normalise();
                    matrix[num2, 0] = vectord[0];
                    matrix[num2, 1] = vectord[1];
                    matrix[num2, 2] = vectord[2];
                    matrix[num2, 3] = plane.Normal.X;
                    matrix[num2, 4] = plane.Normal.Y;
                    matrix[num2, 5] = plane.Normal.Z;
                    matrix[num2, 6] = -1.0;
                    var num3 = Distance3D.PointToCircle(circle, pointd);
                    vector[num2] = num3;
                    num2++;
                }
                var vector2 = matrix.PseudoInverse() * vector;
                if (vector2.Length() < 1E-06)
                {
                    break;
                }
                var origin = circle.Origin;
                origin.X += vector2[0];
                var pointd3 = circle.Origin;
                pointd3.Y += vector2[1];
                var pointd4 = circle.Origin;
                pointd4.Z += vector2[2];
                var normal = circle.Normal;
                normal.X += vector2[3];
                var vectord2 = circle.Normal;
                vectord2.Y += vector2[4];
                var vectord3 = circle.Normal;
                vectord3.Z += vector2[5];
                circle.Radius -= vector2[6];
            }
            CalculateErrors(points, circle);
            return circle;
        }

        public Line3D FitLineToPoints(Collection<Point3D> points)
        {
            if (points == null) throw new MatrixNullReference();

            var origin = Centroid(points);
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0.0;
            var num5 = 0.0;
            var num6 = 0.0;
            foreach (var pointd2 in points)
            {
                var num7 = pointd2.X - origin.X;
                var num8 = pointd2.Y - origin.Y;
                var num9 = pointd2.Z - origin.Z;
                num += num7 * num7;
                num2 += num8 * num8;
                num3 += num9 * num9;
                num4 += num7 * num8;
                num5 += num8 * num9;
                num6 += num7 * num9;
            }
            var mat = new SquareMatrix(3, new[] { num2 + num3, -num4, -num6, -num4, num3 + num, -num5, -num6, -num5, num + num2 });
            var svd = new SVD(mat);
            var direction = new Vector3D(svd.U.GetColumn(svd.SmallestSingularIndex));
            var element = new Line3D(origin, direction);
            CalculateErrors(points, element);
            return element;
        }

        public Plane3D FitPlaneToPoints(Collection<Point3D> points)
        {
            if (points == null) throw new NullReferenceException();

            if (points.Count < 3)
            {
                throw new MatrixException("Not enough points to fit a plane");
            }
            var point = Centroid(points);
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0.0;
            var num5 = 0.0;
            var num6 = 0.0;
            foreach (var pointd2 in points)
            {
                var num7 = pointd2.X - point.X;
                var num8 = pointd2.Y - point.Y;
                var num9 = pointd2.Z - point.Z;
                num += num7 * num7;
                num2 += num8 * num8;
                num3 += num9 * num9;
                num4 += num7 * num8;
                num5 += num8 * num9;
                num6 += num7 * num9;
            }
            var mat = new SquareMatrix(3, new[] { num, num4, num6, num4, num2, num5, num6, num5, num3 });
            var svd = new SVD(mat);
            var normal = new Vector3D(svd.U.GetColumn(svd.SmallestSingularIndex));
            var element = new Plane3D(point, normal);
            CalculateErrors(points, element);
            return element;
        }

        public Sphere3D FitSphereToPoints(Collection<Point3D> points)
        {
            if (points == null) throw new NullReferenceException();

            if (points.Count < 4)
            {
                throw new MatrixException("Need at least 4 points to fit sphere");
            }
            var sphere = new Sphere3D();
            var fitd = new LeastSquaresFit3D();
            sphere.Origin = fitd.Centroid(points);
            sphere.Radius = fitd.RmsError;
            var matrix = new Matrix(points.Count, 4);
            var vector = new Vector(points.Count);
            for (var i = 0; i < 50; i++)
            {
                var num2 = 0;
                foreach (var pointd in points)
                {
                    var vectord = Project3D.PointOntoSphere(sphere, pointd) - sphere.Origin;
                    vectord.Normalise();
                    matrix[num2, 0] = vectord.X;
                    matrix[num2, 1] = vectord.Y;
                    matrix[num2, 2] = vectord.Z;
                    matrix[num2, 3] = -1.0;
                    var num3 = Distance3D.PointToSphere(sphere, pointd);
                    vector[num2] = num3;
                    num2++;
                }
                var vector2 = matrix.PseudoInverse() * vector;
                if (vector2.Length() < 1E-06)
                {
                    break;
                }
                var origin = sphere.Origin;
                origin.X += vector2[0];
                var pointd3 = sphere.Origin;
                pointd3.Y += vector2[1];
                var pointd4 = sphere.Origin;
                pointd4.Z += vector2[2];
                sphere.Radius -= vector2[3];
            }
            CalculateErrors(points, sphere);
            return sphere;
        }

        public TransformationMatrix3D PointToPointMapping(Collection<Point3D> nominal, Collection<Point3D> measured)
        {
            if (measured == null) throw new ArgumentNullException("measured");
            if (nominal == null) throw new ArgumentNullException("nominal");

            if (nominal.Count != measured.Count)
            {
                throw new MatrixException("Number of measured points does not equal number of nominal points");
            }
            var count = nominal.Count;
            var pointd = Centroid(nominal);
            var pointd2 = Centroid(measured);
            var matrix = new Matrix(3, count);
            var matrix2 = new Matrix(count, 3);
            for (var i = 0; i < count; i++)
            {
                var vec = nominal[i] - pointd;
                var vectord2 = measured[i] - pointd2;
                matrix.SetColumn(i, vec);
                matrix2.SetRow(i, vectord2);
            }
            var mat = new Matrix(matrix * matrix2);
            var svd = new SVD(mat);
            var matrix4 = SquareMatrix.Identity(3);
            var num3 = new SquareMatrix(svd.U * svd.VT).Determinant();
            matrix4[2, 2] = num3;
            var rot = new RotationMatrix3D((svd.V * matrix4) * svd.UT);
            var trans = (rot * pointd) - pointd2;
            _transform = new TransformationMatrix3D(trans, rot);
            CalculateErrors(nominal, measured, _transform);
            return _transform;
        }

        public TransformationMatrix3D PointToPointMapping(Collection<Vector3D> nominal, Collection<Vector3D> measured)
        {
            if (nominal.Count != measured.Count)
            {
                throw new MatrixException("Number of measured vectors does not equal number of nominal vectors");
            }
            var list = new Collection<Point3D>();
            var list2 = new Collection<Point3D>();
            foreach (var vectord in nominal)
            {
                list.Add(new Point3D(vectord));
            }
            foreach (var vectord2 in measured)
            {
                list2.Add(new Point3D(vectord2));
            }
            return PointToPointMapping(list, list2);
        }

        private static Vector VectorFromCircle3D(Circle3D initialGuess)
        {
            var vector = new Vector(7);
            vector[0] = initialGuess.Origin.X;
            vector[1] = initialGuess.Origin.Y;
            vector[2] = initialGuess.Origin.Z;
            vector[3] = initialGuess.Normal.X;
            vector[4] = initialGuess.Normal.Y;
            vector[5] = initialGuess.Normal.Z;
            vector[6] = initialGuess.Radius;
            return vector;
        }

        public double AverageError
        {
            get
            {
                return _averageError;
            }
        }

        public Collection<double> Errors
        {
            get
            {
                return _errors;
            }
        }

        public double MaxError
        {
            get
            {
                return _maxError;
            }
        }

        public int PointWithLargestError
        {
            get
            {
                return _pointWithLargestError;
            }
        }

        public double RmsError
        {
            get
            {
                var d = Errors.Sum(num2 => num2*num2);
                d /= Errors.Count;
                return Math.Sqrt(d);
            }
        }

        public double StandardDeviationError
        {
            get
            {
                return _standardDeviationError;
            }
        }

        public TransformationMatrix3D Transform
        {
            get
            {
                return _transform;
            }
        }
    }
}

