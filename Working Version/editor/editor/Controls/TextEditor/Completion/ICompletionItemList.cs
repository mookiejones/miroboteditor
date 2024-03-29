﻿using System.Collections.Generic;
using miRobotEditor.Abstract;
using miRobotEditor.Enums;

namespace miRobotEditor.Interfaces
{
    public interface ICompletionItemList
    {
        IEnumerable<ICompletionItem> Items { get; }
        ICompletionItem SuggestedItem { get; }
        int PreselectionLength { get; }
        bool ContainsAllAvailableItems { get; }

        CompletionItemListKeyResult ProcessInput(char key);

        void Complete(CompletionContext context, ICompletionItem item);
    }
}