using miRobotEditor.Enums;

namespace miRobotEditor.ViewModel
{
    public sealed class LocalVariablesViewModel : ToolViewModel
    {
        public const string ToolContentId = "LocalVariablesTool";

        public LocalVariablesViewModel()
            : base("Local Variables")
        {
            ContentId = "LocalVariablesTool";
            DefaultPane = DefaultToolPane.Bottom;
        }
    }
}