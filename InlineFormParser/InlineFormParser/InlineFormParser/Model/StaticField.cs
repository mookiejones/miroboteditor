#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:15 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class StaticField : StringFieldBase
	{
		public override bool IsVisible => base.IsVisible;

		[XmlIgnore]
		public override bool CanBeCurrent => false;

		public override void Select()
		{
			var visualGroup = Parent as VisualGroup;
			var selectableField = visualGroup?.GetSelectableField(this);
			selectableField?.Select();
		}
	}
}