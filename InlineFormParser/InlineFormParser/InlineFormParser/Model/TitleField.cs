#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:45 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class TitleField : StringFieldBase
	{
		internal TitleField(string title)
			: base(title)
		{
		}

		[XmlIgnore]
		public override bool CanBeCurrent => false;
	}
}