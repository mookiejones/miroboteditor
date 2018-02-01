#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:58 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.ComponentModel;

#endregion

namespace InlineFormParser.Model
{
	public class FieldValueChangingEventArgs : CancelEventArgs
	{
		public FieldValueChangingEventArgs(IField field, string newValue)
		{
			Field = field;
			NewValue = newValue;
		}

		public IField Field { get; }

		public int FieldIndex => Field.ChildIndex + 1;

		public string PreviousValue => Field.TPValueString;

		public string NewValue { get; }

		public string TPString => $"%P {Field.FieldId}:{NewValue}";
	}
}