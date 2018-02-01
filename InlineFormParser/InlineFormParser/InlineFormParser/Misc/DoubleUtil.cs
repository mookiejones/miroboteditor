using System;
using System.Runtime.InteropServices;
using System.Windows;
namespace InlineFormParser.Misc
{

public static class DoubleUtil
{
	[StructLayout(LayoutKind.Explicit)]
	private struct NanUnion
	{
		[FieldOffset(0)]
		internal readonly ulong UintValue;

		[FieldOffset(0)]
		internal double DoubleValue;
	}

	internal const double DBL_EPSILON = 2.2204460492503131E-16;

	internal const float FLT_MIN = 1.17549435E-38f;

	public static bool AreClose(double value1, double value2)
	{
		if (value1 == value2)
		{
			return true;
		}
		double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
		double num2 = value1 - value2;
		if (0.0 - num < num2)
		{
			return num > num2;
		}
		return false;
	}

	public static bool AreClose(Point point1, Point point2)
	{
		if (AreClose(point1.X, point2.X))
		{
			return AreClose(point1.Y, point2.Y);
		}
		return false;
	}

	public static bool AreClose(Size size1, Size size2)
	{
		if (AreClose(size1.Width, size2.Width))
		{
			return AreClose(size1.Height, size2.Height);
		}
		return false;
	}

	public static bool GreaterThan(double value1, double value2)
	{
		if (value1 > value2)
		{
			return !AreClose(value1, value2);
		}
		return false;
	}

	public static bool GreaterThanOrClose(double value1, double value2)
	{
		if (!(value1 > value2))
		{
			return AreClose(value1, value2);
		}
		return true;
	}

	public static bool IsNaN(double value)
	{
		NanUnion nanUnion = default(NanUnion);
		nanUnion.DoubleValue = value;
		NanUnion nanUnion2 = nanUnion;
		ulong num = (ulong)((long)nanUnion2.UintValue & -4503599627370496L);
		ulong num2 = nanUnion2.UintValue & 0xFFFFFFFFFFFFF;
		if (num != 9218868437227405312L && num != 18442240474082181120uL)
		{
			return false;
		}
		return num2 != 0;
	}

	public static bool IsOne(double value)
	{
		return Math.Abs(value - 1.0) < 2.2204460492503131E-15;
	}

	public static bool LessThan(double value1, double value2)
	{
		if (value1 < value2)
		{
			return !AreClose(value1, value2);
		}
		return false;
	}

	public static bool LessThanOrClose(double value1, double value2)
	{
		if (!(value1 < value2))
		{
			return AreClose(value1, value2);
		}
		return true;
	}
}
}
