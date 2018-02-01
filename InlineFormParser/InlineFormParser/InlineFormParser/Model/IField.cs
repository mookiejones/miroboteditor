#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:36 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Model
{
	public interface IField
	{
		int ChildIndex { get; }

		string FieldId { get; }

		string TPValueString { get; }

		string Label { get; set; }

		string Unit { get; set; }

		bool EnableInput { get; set; }

		bool IsVisible { get; set; }

		string Key { get; set; }
	}
}