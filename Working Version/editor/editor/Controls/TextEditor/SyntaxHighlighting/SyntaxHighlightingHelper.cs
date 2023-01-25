namespace miRobotEditor.Controls.TextEditor.SyntaxHighlighting
{
    internal sealed class SyntaxHighlightingHelper
    {
        public string GetFilename(string name) => $"{GetType().Namespace}.{name}Highlight.xshd";

        public static SyntaxHighlightingHelper Create() => new SyntaxHighlightingHelper();
    }
}
