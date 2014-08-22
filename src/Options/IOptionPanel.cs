﻿namespace miRobotEditor.Options
{
    /// <summary>
    ///     Provides access to objects containing OptionBindings, such as OptionPanels.
    /// </summary>
    public interface IOptionPanel
    {
        /// <summary>
        ///     Gets/sets the owner (the context object used when building the option panels
        ///     from the addin-tree). This is null for IDE options or the IProject instance for project options.
        /// </summary>
        object Owner { get; set; }

        object Control { get; }

        void LoadOptions();
        bool SaveOptions();
    }
}