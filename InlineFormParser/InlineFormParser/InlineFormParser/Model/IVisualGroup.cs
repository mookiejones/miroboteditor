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

#region Usings

using System.Collections.Generic;

#endregion

namespace InlineFormParser.Model
{
	public interface IVisualGroup
	{
		bool HasFields { get; }

		Field[] Fields { get; }

		List<Field> VisibleFields { get; }

		bool IsVisible { get; }

		bool CanBeCurrent { get; }

		bool IsSelected { get; }

		void UpdateIsSelected();
	}
}