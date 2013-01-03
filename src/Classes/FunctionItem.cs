using System.ComponentModel;
using System.Text;

namespace miRobotEditor.Classes
{
    public class FunctionItem
    {
        public string Text { get;  set; }
        public string Name { get;  set; }
        public string Returns { get;  set; }
        public string Parameters { get;  set; }

      
        public int Offset { get; set; }

        [Localizable(false)]
        public string Tooltip
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("Name:= " + Name);
                if (!(string.IsNullOrEmpty(Returns))) sb.AppendLine("Returns:= " + Returns);
                if (!(string.IsNullOrEmpty(Parameters))) sb.AppendLine("Parameters:= " + Parameters);
                return sb.ToString();
            }

        }
        public FunctionItem()
        {
        }

        public FunctionItem(string text, string name, string returns, string parameters,int offset)
        {
            Text = text;
            Name = name;
            Returns = returns;
            Parameters = parameters;
            Offset = offset;
        }
        public override string ToString()
        {
            return Text;
        }

    }
}
