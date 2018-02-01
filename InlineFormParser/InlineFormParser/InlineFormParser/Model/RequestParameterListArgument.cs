using System.Runtime.InteropServices;

namespace InlineFormParser.Model
{
	public struct RequestParameterListArgument
	{
		public int FieldIndex;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string ParamListHandle;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string Value;
	}
}