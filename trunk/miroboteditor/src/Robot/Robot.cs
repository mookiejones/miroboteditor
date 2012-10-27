namespace miRobotEditor.Robot
{
    public class RobotClass
    {
        private static AbstractRobotClass _instance;
        public static AbstractRobotClass Instance { get { return _instance; } set { _instance = value; } }
    }
}
