﻿using miRobotEditor.Interfaces;
using miRobotEditor.Languages;

//TODO Unused

namespace miRobotEditor.GUI.Editor

{
// ReSharper disable UnusedMember.Global
    public class DefaultCompletionItem : ICompletionItem
    {
        public DefaultCompletionItem(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
        public virtual string Description { get; set; }
        public virtual IImage Image { get; set; }

        public virtual double Priority
        {
            get { return 0; }
        }

        public virtual void Complete(CompletionContext context)
        {
            context.Editor.Document.Replace(context.StartOffset, context.Length, Text);
            context.EndOffset = context.StartOffset + Text.Length;
        }
    }

    // ReSharper restore UnusedMember.Global
}