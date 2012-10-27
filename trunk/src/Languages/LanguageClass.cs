namespace miRobotEditor.Languages
{
    public class LanguageClass
    {
        private static AbstractLanguageClass _instance;
        public static AbstractLanguageClass Instance { get { return _instance; } set { _instance = value; } }
    }
}
