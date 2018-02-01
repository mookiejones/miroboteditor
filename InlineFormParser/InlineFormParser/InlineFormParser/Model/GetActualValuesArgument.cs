using System.Runtime.InteropServices;

namespace InlineFormParser.Model
{
	public struct GetActualValuesArgument
	{
		public bool ModifiedOnly;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string TPString;
	}
}