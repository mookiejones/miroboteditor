﻿using System;

namespace miRobotEditor.GUI.Editor
{
    /// <summary>
    ///     Base interface for IInsightWindow and ICompletionListWindow.
    /// </summary>
    public interface ICompletionListWindow
    {
        /// <summary>
        ///     Gets/Sets the width of the window.
        ///     double.NaN is used to represent automatic width.
        ///     For the completion list window default width is a fixed number - using automatic width
        ///     will reduce performance when a large number of items is shown.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        ///     Gets/Sets the height of the window.
        ///     double.NaN is used to represent automatic height.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        ///     Gets/Sets whether the window should close automatically.
        ///     The default value is true.
        /// </summary>
        bool CloseAutomatically { get; set; }

        /// <summary>
        ///     Gets/Sets the start of the text range in which the window stays open.
        ///     Has no effect if CloseAutomatically is false.
        /// </summary>
        int StartOffset { get; set; }

        /// <summary>
        ///     Gets/Sets the end of the text range in which the window stays open.
        ///     Has no effect if CloseAutomatically is false.
        /// </summary>
        int EndOffset { get; set; }

        /// <summary>
        ///     Closes the window.
        /// </summary>
// ReSharper disable UnusedMember.Global
        void Close();

        /// <summary>
        ///     Occurs after the window was closed.
        /// </summary>
        event EventHandler Closed;

        // ReSharper restore UnusedMember.Global
    }
}