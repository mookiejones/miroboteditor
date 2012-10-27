using System.Text;

namespace miRobotEditor.Classes
{
    public class FunctionItem
    {
        private readonly string _text;
        private readonly string _name;
        private readonly string _returns;
        private readonly string _parameters;
        public string Text
        {
            get { return _text; }
        }
        public string Tooltip
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("Name:= " + _name);
                if (!(string.IsNullOrEmpty(_returns))) sb.AppendLine("Returns:= " + _returns);
                if (!(string.IsNullOrEmpty(_parameters))) sb.AppendLine("Parameters:= " + _parameters);
                return sb.ToString();
            }

        }
        public FunctionItem(string text, string name, string returns, string parameters)
        {
            _text = text;
            _name = name;
            _returns = returns;
            _parameters = parameters;
        }
        public override string ToString()
        {
            return _text;
        }

    }
}
