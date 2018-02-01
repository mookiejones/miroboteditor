using System.Runtime.InteropServices;

namespace InlineFormParser.Model
{
	public struct FieldValueChangingArgument
	{
		public int FieldIndex;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string NewValue;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string TPString;

		[MarshalAs(UnmanagedType.Bool)]
		public bool Cancel;
	}
}