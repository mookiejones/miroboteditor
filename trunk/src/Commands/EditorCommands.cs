using System.Windows.Input;

namespace miRobotEditor.Commands
{
    public static class EditorCommands
    {
        public static readonly RoutedCommand ToggleComment = new RoutedCommand("ToggleComment",typeof(EditorCommands));
        public static readonly RoutedCommand Goto = new RoutedCommand("Goto",typeof(EditorCommands));
        public static readonly RoutedCommand Save = new RoutedCommand("Save",typeof(EditorCommands));
        public static readonly RoutedCommand ToggleFolding = new RoutedCommand("ToggleFolding", typeof(EditorCommands));
        public static readonly RoutedCommand ToggleAllFolding = new RoutedCommand("ToggleAllFolding", typeof(EditorCommands));
        public static readonly RoutedCommand ShowDefinitions = new RoutedCommand("ShowDefinitions", typeof(EditorCommands));
        public static readonly RoutedCommand OpenAllFolds = new RoutedCommand("OpenAllFolds", typeof(EditorCommands));
        public static readonly RoutedCommand CloseAllFolds = new RoutedCommand("CloseAllFolds", typeof(EditorCommands));
        public static readonly RoutedCommand IncreaseIndent = new RoutedCommand("IncreaseIndent", typeof(EditorCommands));
        public static readonly RoutedCommand DecreaseIndent = new RoutedCommand("DecreaseIndent", typeof(EditorCommands));
    }
}
