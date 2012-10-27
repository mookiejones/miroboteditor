using ISTUK.MathLibrary;

namespace miRobotEditor.GUI.AngleConverter.Robot
{
    public class Axis
    {
      
        public Axis()
        {
        }

        public Axis(Vector3D point, Vector3D direction)
        {
            Point = new Vector3D(point);
            Direction = new Vector3D(direction);
            Direction.Normalise();
        }

        public Vector3D Direction { get; set; }


        public Vector3D Point { get; set; }

    }
}

