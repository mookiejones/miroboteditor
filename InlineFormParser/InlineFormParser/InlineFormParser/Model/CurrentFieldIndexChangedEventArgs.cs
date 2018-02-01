#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:18 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.ComponentModel;

#endregion

namespace InlineFormParser.Model
{
	public class CurrentFieldIndexChangedEventArgs : CancelEventArgs
	{
		public CurrentFieldIndexChangedEventArgs(IField prevField, IField newField)
		{
			PreviousField = prevField;
			NewField = newField;
		}

		public IField PreviousField { get; }

		public int PreviousFieldIndex
		{
			get
			{
				if (PreviousField == null) return 0;
				return PreviousField.ChildIndex + 1;
			}
		}

		public IField NewField { get; }

		public int NewFieldIndex => NewField.ChildIndex + 1;
	}
}