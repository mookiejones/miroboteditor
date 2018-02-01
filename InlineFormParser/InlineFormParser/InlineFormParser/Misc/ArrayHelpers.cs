#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:36 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace InlineFormParser.Misc
{
	public static class ArrayHelpers
	{
		[Flags]
		public enum CheckStringArrayOptions
		{
			None = 0,
			NotNull = 0,
			NotEmpty = 1,
			ElementsNotNull = 2,
			ElementsNotEmpty = 4,
			ElementsUnique = 8,
			CheckElements = 0xE,
			CheckAll = 0xF
		}

		public static string ArrayToString(Array array, bool recursive)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (array == null)
			{
				stringBuilder.Append("null");
			}
			else
			{
				stringBuilder.Append('{');
				bool flag = true;
				foreach (object item in array)
				{
					if (!flag) stringBuilder.Append(", ");
					if (item != null)
						if (recursive && item is Array)
							stringBuilder.Append(ArrayToString((Array) item, true));
						else
							stringBuilder.Append(item);
					else
						stringBuilder.Append("null");
					flag = false;
				}

				stringBuilder.Append('}');
			}

			return stringBuilder.ToString();
		}

		public static string ArrayToString(Array array)
		{
			return ArrayToString(array, true);
		}

		public static void CheckStringArray(string[] array, string arrayName, CheckStringArrayOptions options)
		{
			if (string.IsNullOrEmpty(arrayName)) throw new ArgumentNullException(nameof(arrayName));
			if (array == null) throw new ArgumentNullException($"{arrayName} is null");
			if ((options & CheckStringArrayOptions.NotEmpty) != 0 && array.Length == 0)
				throw new ArgumentException($"{arrayName} is empty");
			if ((options & CheckStringArrayOptions.CheckElements) == CheckStringArrayOptions.None) return;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if ((options & CheckStringArrayOptions.ElementsNotNull) != 0 && text == null)
					throw new ArgumentNullException($"{arrayName}[{i}] is null");
				if ((options & CheckStringArrayOptions.ElementsNotEmpty) != 0 && text == string.Empty)
					throw new ArgumentException($"{arrayName}[{i}] is empty");
			}

			if ((options & CheckStringArrayOptions.ElementsUnique) == CheckStringArrayOptions.None) return;
			HashSet<string> hashSet = new HashSet<string>();
			int num = 0;
			string text2;
			while (true)
			{
				if (num < array.Length)
				{
					text2 = array[num];
					if (text2 != null)
					{
						if (hashSet.Contains(text2)) break;
						hashSet.Add(text2);
					}

					num++;
					continue;
				}

				return;
			}

			throw new ArgumentException($"Duplicate element \"{text2}\" in array {arrayName}");
		}
	}
}