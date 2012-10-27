namespace miRobotEditor.GUI.ExplorerWindow
{
    public abstract class EnumDisplayEntry
    {
        protected EnumDisplayEntry(string enumValue, string displayString, bool excludeFromDisplay)
        {
            ExcludeFromDisplay = excludeFromDisplay;
            DisplayString = displayString;
            EnumValue = enumValue;
        }

        public string EnumValue { get; set; }
        public string DisplayString { get; set; }
        public bool ExcludeFromDisplay { get; set; }
    }
}