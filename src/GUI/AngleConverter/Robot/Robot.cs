using System.Collections.ObjectModel;

namespace miRobotEditor.GUI.AngleConverter.Robot
{
    public class Robot
    {
        private readonly Collection<double> _jointPositions;
        private TransformationMatrix3D _toolframe;

        public Robot()
        {
            Joints = new Collection<Joint>();
            _jointPositions = new Collection<double>();
        }

        private Collection<Joint> Joints { get; set; }

        private TransformationMatrix3D ToolFrame
        {
            get { return _toolframe; }
            set { _toolframe = new TransformationMatrix3D(value); }
        }

        public static Robot AbsoluteToRelative(Robot absoluteModel)
        {
            var robot = new Robot();
            robot.AddJoint(new Joint(absoluteModel.Joints[0]));
            for (int i = 1; i < absoluteModel.Joints.Count; i++)
            {
                robot.AddJoint(
                    new Joint(absoluteModel.Joints[i - 1].Transform.Inverse()*absoluteModel.Joints[i].Transform));
            }
            robot.ToolFrame = absoluteModel.Joints[absoluteModel.Joints.Count - 1].Transform.Inverse()*
                              absoluteModel.ToolFrame;
            return robot;
        }

        public void AddJoint(Joint joint)
        {
            Joints.Add(joint);
            _jointPositions.Add(0.0);
        }

        public void SetJointPosition(int joint, double pos)
        {
            _jointPositions[joint] = pos;
        }

        public void SetJoints(params double[] jointPositions)
        {
            if (jointPositions.Length != Joints.Count)
            {
                throw new MatrixException("Wrong number of arguments");
            }
            for (int i = 0; i < jointPositions.Length; i++)
            {
                SetJointPosition(i, jointPositions[i]);
            }
        }
    }
}