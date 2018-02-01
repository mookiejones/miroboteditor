#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:19 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Model
{
	public class RequestParameterListEventArgs : EventArgs
	{
		internal RequestParameterListEventArgs(ParamListField paramListField)
		{
			ParamListField = paramListField;
		}

		public ParamListField ParamListField { get; }

		public string ParamListHandle => ParamListField.ParamListHandle;

		public int FieldIndex => ParamListField.ChildIndex + 1;

		public string FieldValue => ParamListField.ShownValue;
	}
}