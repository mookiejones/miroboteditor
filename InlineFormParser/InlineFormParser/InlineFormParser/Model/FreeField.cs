#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:14 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.ComponentModel;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class FreeField : LimitedStringFieldBase
	{
		[XmlAttribute("InputPattern")]
		[Description("A regular expression for input validation")]
		public string InputPatternAttribute
		{
			get => inputPattern;
			set => inputPattern = value;
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			if (string.IsNullOrEmpty(inputPattern)) inputPattern = maxLength >= 1 ? $"^.{{0,{maxLength}}}$" : "^.*$";
		}
	}
}