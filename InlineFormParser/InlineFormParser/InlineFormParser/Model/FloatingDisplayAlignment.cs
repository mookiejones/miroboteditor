using System;

namespace InlineFormParser.Model
{
	[Flags]
	public enum FloatingDisplayAlignment
	{
		HCenter = 0,
		Left = 1,
		Right = 2,
		HorizontalMask = 3,
		InvalidHorizontal = 3,
		VCenter = 0,
		Top = 0x10,
		Bottom = 0x20,
		VerticalMask = 0x30,
		InvalidVertical = 0x30
	}
}