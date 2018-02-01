#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:09 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Model
{
	public class NameField : LimitedStringFieldBase
	{
		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			if (maxLength < 1) throw new ArgumentException("MaxLength must be >= 1");
			inputPattern = $"^[a-zA-Z$][a-zA-Z0-9_]{{0,{maxLength - 1}}}$";
		}
	}
}