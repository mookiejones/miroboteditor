using System;
using System.Collections.Generic;

namespace miRobotEditor.Languages
{
    public interface IPosition
    {
         string RawValue { get; set; }
         string Scope { get; set; }
         string Name { get; set; }
         string Type { get; set; }
         List<PositionValue>PositionalValues{get;set;}
         void ParseValues();
         string ExtractFromMatch();
       
    }
}
