#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:59 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	internal static class SharedUtils
	{
		public static int URShift(int number, int bits)
		{
			return (int) ((uint) number >> bits);
		}

		public static int ReadInput(TextReader sourceTextReader, byte[] target, int start, int count)
		{
			if (target.Length == 0) return 0;
			char[] array = new char[target.Length];
			int num = sourceTextReader.Read(array, start, count);
			if (num == 0) return -1;
			for (int i = start; i < start + num; i++) target[i] = (byte) array[i];
			return num;
		}

		internal static byte[] ToByteArray(string sourceString)
		{
			return Encoding.UTF8.GetBytes(sourceString);
		}

		internal static char[] ToCharArray(byte[] byteArray)
		{
			return Encoding.UTF8.GetChars(byteArray);
		}
	}
}