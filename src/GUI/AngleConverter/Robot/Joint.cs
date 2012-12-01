using ISTUK.MathLibrary;

namespace miRobotEditor.GUI.AngleConverter.Robot
{
    public class Joint
    {
       
        public Joint(Joint joint)
        {
            Transform = new TransformationMatrix3D(joint.Transform);
        }

        public Joint(TransformationMatrix3D mat)
        {
            Transform= mat;
        }

        public TransformationMatrix3D Transform
        {
            get; private set; }
    }
}

