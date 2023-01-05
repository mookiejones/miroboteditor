using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace miRobotEditor.Languages
{
    public class PositionBase : IPosition
    {
        private readonly ReadOnlyObservableCollection<PositionValue> _positionalValues = null;
        private ObservableCollection<PositionValue> _values = new ObservableCollection<PositionValue>();

        public PositionBase(string value)
        {
            RawValue = value;
            ParseValues();
        }

        public string Type { get; set; }
        public string RawValue { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }
        //  public List<PositionValue> PositionalValues { get; set; }

        public ReadOnlyObservableCollection<PositionValue> PositionalValues => _positionalValues ?? new ReadOnlyObservableCollection<PositionValue>(_values);


        public void ParseValues()
        {
            try
            {
                _values = new ObservableCollection<PositionValue>();
                string[] sp = RawValue.Split('=');
                string[] decl = sp[1].Substring(1, sp[1].Length - 2).Split(',');

                foreach (var ss in decl.Select(s => s.Split(' ')))
                {
                    _values.Add(new PositionValue {Name = ss[0], Value = ss[1]});
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
// ReSharper restore EmptyGeneralCatchClause
        }

        [Localizable(false)]
        public string ExtractFromMatch()
        {
            string result = string.Empty;
            try
            {
                result = PositionalValues.Aggregate(result,
                    (current, v) => current + String.Format("{0} {1},", v.Name, v.Value));

                return result.Substring(0, result.Length - 1);
            }
            catch
            {
                return string.Empty;
            }
        }

        public override string ToString()
        {
            return RawValue;
        }

// ReSharper disable UnusedMember.Local
        private string ConvertFromHex(string value)
// ReSharper restore UnusedMember.Local
        {
            double d = double.Parse(value.Substring(1, value.Length - 2), NumberStyles.HexNumber);
            return Convert.ToString(d);
        }

// ReSharper disable UnusedMember.Local
        private bool IsNumeric(string value)
// ReSharper restore UnusedMember.Local
        {
            try
            {
#pragma warning disable 168
                double v = Convert.ToDouble(value);
#pragma warning restore 168
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}