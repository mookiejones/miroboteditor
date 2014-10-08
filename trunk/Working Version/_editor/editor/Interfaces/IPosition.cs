using System.Collections.Generic;
using miRobotEditor.Classes;

namespace miRobotEditor.Interfaces
{
    public interface IPosition
    {
        string RawValue { get; set; }
        string Scope { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        IEnumerable<PositionValue> PositionalValues { get; }
        void ParseValues();
        string ExtractFromMatch();
    }
}