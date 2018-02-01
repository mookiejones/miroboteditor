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

using System;

#endregion

namespace InlineFormParser.Model
{
	[Flags]
	public enum PopupKeyboardTypes
	{
		MaskType = 0xFF,
		MaskTypeOptions = 0xFF00,
		MaskCommonOptions = 0xFF0000,
		MaskAll = 0xFFFFFF,
		Default = 0,
		AlphaNumeric = 1,
		Numeric = 2,
		NoKeyboard = 0xFE,
		AutoDetect = 0xFF,
		NoTypeOptions = 0,
		NumericOptionNoDot = 0x100,
		NumericOptionNoSign = 0x200,
		NumericOptionUpDownEnabled = 0x400,
		NumericOptionUpDownIncDec = 0x800,
		NoCommonOptions = 0,
		CommonOptionObserveBounds = 0x10000,
		NumericInt = 0xD02,
		NumericUInt = 0xF02,
		NumericFloat = 2,
		NumericUFloat = 0x202
	}
}