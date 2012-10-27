using ISTUK.MathLibrary;
using System;
using System.Collections.ObjectModel;

namespace miRobotEditor.GUI.AngleConverter.Robot
{
    public class RobotBaseAndToolCalibration
    {
        private TransformationMatrix3D calculatedRobotBase;
        private TransformationMatrix3D calculatedRobotTool;
        private double conditionNumber;

        public void CalibrateRobotBaseAndTool(Collection<TransformationMatrix3D> RobotPoses, Collection<TransformationMatrix3D> MeasuredPoses)
        {
            if (RobotPoses.Count != MeasuredPoses.Count)
            {
                throw new ArgumentException("Number of measured poses does not equal the number of robot poses");
            }
            if (RobotPoses.Count < 3)
            {
                throw new MatrixException("Number of robot poses must be 3 or more");
            }
            Collection<Matrix> list = new Collection<Matrix>();
            Collection<Matrix> list2 = new Collection<Matrix>();
            for (var i = 0; i < MeasuredPoses.Count; i++)
            {
                TransformationMatrix3D matrixd = MeasuredPoses[i];
                TransformationMatrix3D matrixd2 = RobotPoses[i];
                Quaternion rotation = (Quaternion) matrixd.Rotation;
                Quaternion quaternion2 = (Quaternion) matrixd2.Rotation;
                Vector3D vectord = rotation.Vector;
                double[] elements = new double[9];
                elements[1] = -vectord.Z;
                elements[2] = vectord.Y;
                elements[3] = vectord.Z;
                elements[5] = -vectord.X;
                elements[6] = -vectord.Y;
                elements[7] = vectord.X;
                SquareMatrix matrix = new SquareMatrix(3, elements);
                Vector3D vectord2 = quaternion2.Vector;
                double[] numArray2 = new double[9];
                numArray2[1] = -vectord2.Z;
                numArray2[2] = vectord2.Y;
                numArray2[3] = vectord2.Z;
                numArray2[5] = -vectord2.X;
                numArray2[6] = -vectord2.Y;
                numArray2[7] = vectord2.X;
                SquareMatrix matrix2 = new SquareMatrix(3, numArray2);
                Matrix matrix3 = (Matrix) ((vectord * vectord.Transpose()) / rotation.Scalar);
                Matrix matrix4 = (Matrix) ((vectord * vectord2.Transpose()) / rotation.Scalar);
                SquareMatrix matrix5 = new SquareMatrix(((Matrix) ((rotation.Scalar * SquareMatrix.Identity(3)) + matrix)) + matrix3);
                SquareMatrix mat = new SquareMatrix(((Matrix) ((-quaternion2.Scalar * SquareMatrix.Identity(3)) + matrix2)) - matrix4);
                list.Add(matrix5.Augment(mat));
                list2.Add((Matrix) (vectord2 - ((quaternion2.Scalar / rotation.Scalar) * vectord)));
            }
            Matrix matrix7 = new Matrix(list[0].Transpose());
            Matrix matrix8 = new Matrix(list2[0].Transpose());
            for (var j = 1; j < list.Count; j++)
            {
                matrix7 = matrix7.Augment(list[j].Transpose());
                matrix8 = matrix8.Augment(list2[j].Transpose());
            }
            matrix7 = matrix7.Transpose();
            matrix8 = matrix8.Transpose();
            conditionNumber = matrix7.ConditionNumber();
            Vector vector = new Vector(matrix7.PseudoInverse() * matrix8);
            double scalar = Math.Pow(((1.0 + (vector[3] * vector[3])) + (vector[4] * vector[4])) + (vector[5] * vector[5]), -0.5);
            Vector3D vectord3 = (Vector3D) (new Vector3D(vector[3], vector[4], vector[5]) * scalar);
            Vector3D vectord4 = (Vector3D) (new Vector3D(vector[0], vector[1], vector[2]) * scalar);
            double num4 = Math.Pow(((1.0 - (vectord4[0] * vectord4[0])) - (vectord4[1] * vectord4[1])) - (vectord4[2] * vectord4[2]), 0.5);
            Quaternion quaternion3 = new Quaternion(vectord3, scalar);
            RotationMatrix3D rot = ((RotationMatrix3D) quaternion3).Inverse();
            Quaternion quaternion4 = new Quaternion(vectord4, num4);
            RotationMatrix3D matrixd4 = (RotationMatrix3D) quaternion4;
            double num5 = 0.0;
            for (var k = 0; k < RobotPoses.Count; k++)
            {
                Quaternion quaternion5 = (Quaternion) MeasuredPoses[0].Rotation;
                Quaternion quaternion6 = (Quaternion) RobotPoses[0].Rotation;
                num5 += (Vector.Dot((Vector) (quaternion5.Vector / quaternion5.Scalar), quaternion4.Vector) + ((quaternion6.Scalar / quaternion5.Scalar) * quaternion3.Scalar)) - Vector.Dot((Vector) (quaternion6.Vector / quaternion5.Scalar), quaternion3.Vector);
            }
            num5 /= (double) RobotPoses.Count;
            if (Math.Sign(num5) != Math.Sign(num4))
            {
                num4 = -num4;
            }
            Collection<Matrix> list3 = new Collection<Matrix>();
            Collection<Vector> list4 = new Collection<Vector>();
            for (var m = 0; m < RobotPoses.Count; m++)
            {
                Matrix matrix9 = new RotationMatrix3D(MeasuredPoses[m].Rotation);
                list3.Add(matrix9.Augment(-SquareMatrix.Identity(3)));
                list4.Add(new Vector3D((rot * RobotPoses[m].Translation) - MeasuredPoses[m].Translation));
            }
            Matrix matrix10 = new Matrix(list3[0].Transpose());
            Matrix matrix11 = new Matrix(list4[0].Transpose());
            for (var n = 1; n < list3.Count; n++)
            {
                matrix10 = matrix10.Augment(list3[n].Transpose());
                matrix11 = matrix11.Augment(list4[n].Transpose());
            }
            matrix10 = matrix10.Transpose();
            matrix11 = matrix11.Transpose();
            conditionNumber = Math.Max(conditionNumber, matrix10.ConditionNumber());
            Vector vector2 = new Vector(matrix10.PseudoInverse() * matrix11);
            calculatedRobotBase = new TransformationMatrix3D(new Vector3D(vector2[3], vector2[4], vector2[5]), rot);
            calculatedRobotTool = new TransformationMatrix3D(new Vector3D(vector2[0], vector2[1], vector2[2]), matrixd4.Inverse());
        }

        public TransformationMatrix3D CalculatedRobotBase
        {
            get
            {
                return calculatedRobotBase;
            }
        }

        public TransformationMatrix3D CalculatedRobotTool
        {
            get
            {
                return calculatedRobotTool;
            }
        }

        public double ConditionNumber
        {
            get
            {
                return conditionNumber;
            }
        }
    }
}

