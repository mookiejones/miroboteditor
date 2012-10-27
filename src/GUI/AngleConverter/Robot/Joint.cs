using ISTUK.MathLibrary;

namespace miRobotEditor.GUI.AngleConverter.Robot
{
    public class Joint
    {
        private readonly TransformationMatrix3D transform;

        public Joint(Joint joint)
        {
            transform = new TransformationMatrix3D(joint.Transform);
        }

        public Joint(TransformationMatrix3D mat)
        {
            transform = mat;
        }

        public TransformationMatrix3D Transform
        {
            get
            {
                return transform;
            }
        }
    }
}

