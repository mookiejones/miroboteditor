//-----------------------------------------------------------------------
// <copyright file="SingleInstance.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     This class checks to make sure that only one instance of 
//     this application is running at a time.
// </summary>
//-----------------------------------------------------------------------

using miRobotEditor.GUI.Editor;
using miRobotEditor.ViewModel;

namespace miRobotEditor
{
    /// <summary>
    ///     Trying to Only Have one _instance of Each here
    /// </summary>
    internal class InstanceOf
    {
        private static AvalonEditor _ieditor;

        private static DocumentViewModel _idocument;

        public static AvalonEditor Editor
        {
            get { return _ieditor ?? (_ieditor = new AvalonEditor()); }
            set { _ieditor = value; }
        }

        public static DocumentViewModel Document
        {
            get { return _idocument ?? (_idocument = new DocumentViewModel(null)); }
            set { _idocument = value; }
        }
    }

   
}