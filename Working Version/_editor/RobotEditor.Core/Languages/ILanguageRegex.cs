﻿using System.Text.RegularExpressions;
namespace RobotEditor.Core.Languages
{
    public interface ILanguageRegex
    {
        Regex MethodRegex { get; }
        Regex FieldRegex { get; }
        Regex EnumRegex { get; }
        Regex XYZRegex { get; }
        Regex StructRegex { get; }
        Regex SignalRegex { get; }
    }
}
