namespace miRobotEditor.ViewModel
{
    public class LocalVariablesViewModel:ToolViewModel
    {
        public const string ToolContentId = "LocalVariablesTool";

        public LocalVariablesViewModel() : base("Local Variables")
        {
            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Bottom;
          //  IconSource = Utilities.GetIcon(Global.IconProperty);
        }

    }
}
