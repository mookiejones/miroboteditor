using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miRobotEditor.Languages
{
    class PositionBase:IPosition
    {
        public PositionBase(string value)
        {           
            RawValue = value;
            ParseValues();
        }
       
        public override string ToString()
        {
            return RawValue;
        }

        public string Type { get; set; }
        public string RawValue { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }
        public List<PositionValue> PositionalValues { get; set; }
        public void ParseValues()
        {
            PositionalValues = new List<PositionValue>();
            var sp = RawValue.Split('=');
            var decl = sp[1].Substring(1, sp[1].Length - 2).Split(',');

            foreach (var ss in decl.Select(s => s.Split(' ')))
            {
                PositionalValues.Add(new PositionValue { Name = ss[0], Value = ss[1]});
            }
        }

        private string ConvertFromHex(string value)
        {
            var d = double.Parse(value.Substring(1, value.Length - 2), System.Globalization.NumberStyles.HexNumber);
            return Convert.ToString(d);
        }

        private bool IsNumeric(string value)
        {
            try
            {
                var v = Convert.ToDouble(value);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public string ExtractFromMatch()
        {
            var result = string.Empty; ;          
            foreach (var v in PositionalValues)
                result += String.Format("{0} {1},", v.Name, v.Value);

            return result.Substring(0, result.Length - 1);
        }
    }
    public class PositionValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
