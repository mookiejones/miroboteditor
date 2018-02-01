#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:36 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#endregion

namespace InlineFormParser.Model
{
	public static class Helpers
	{
		private static char[] invalidModuleNameChars;

		public static string DeveloperRegionCode => "DEV";

		public static char[] InvalidModuleNameCharacters
		{
			get
			{
				if (invalidModuleNameChars == null)
				{
					var list = new List<char>();
					list.AddRange(Path.GetInvalidFileNameChars());
					list.Add('.');
					list.Add('_');
					invalidModuleNameChars = list.ToArray();
				}

				return invalidModuleNameChars;
			}
		}

		public static bool TryReplaceParameters(string input, out string output)
		{
			var num = input.IndexOf('%');
			if (num < 0)
			{
				output = input;
				return true;
			}

			var result = true;
			var num2 = num;
			var stringBuilder = new StringBuilder(input.Substring(0, num));
			while (num2 < input.Length)
			{
				num = input.IndexOf('%', num2);
				if (num >= 0)
				{
					var num3 = num + 1;
					int i;
					for (i = num3; i < input.Length && char.IsDigit(input[i]); i++)
					{
					}

					if (i > num3)
					{
						var s = input.Substring(num3, i - num3);
						var num4 = default(int);
						if (int.TryParse(s, out num4) && num4 > 0)
						{
							if (num > num2) stringBuilder.Append(input.Substring(num2, num - num2));
							stringBuilder.AppendFormat("{{{0}}}", num4 - 1);
							num2 = i;
						}
						else
						{
							result = false;
						}
					}

					if (i > num2)
					{
						stringBuilder.Append(input.Substring(num2, i - num2));
						num2 = i;
					}
				}
				else
				{
					stringBuilder.Append(input.Substring(num2));
					num2 = input.Length;
				}
			}

			output = stringBuilder.ToString();
			return result;
		}

		public static string NormalizeWhitespaces(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));
			if (input.IndexOf('\n') < 0 && input.IndexOf('\t') < 0 && input.IndexOf("  ") < 0) return input.Trim();
			var array = input.Split('\n');
			var stringBuilder = new StringBuilder();
			var array2 = array;
			foreach (var text in array2)
			{
				var text2 = text.Trim(' ', '\t', '\n');
				if (text2.Length > 0)
				{
					var array3 = text2.Split(' ', '\t');
					var stringBuilder2 = new StringBuilder();
					var array4 = array3;
					foreach (var text3 in array4)
						if (text3.Length > 0)
						{
							if (stringBuilder2.Length > 0) stringBuilder2.Append(' ');
							stringBuilder2.Append(text3);
						}

					if (stringBuilder2.Length > 0)
					{
						if (stringBuilder.Length > 0) stringBuilder.AppendLine();
						stringBuilder.Append(stringBuilder2);
					}
				}
			}

			return stringBuilder.ToString();
		}

		public static string GetLanguageCode(string code)
		{
			var num = code.IndexOf('-');
			if (num < 0) return code;
			return code.Substring(0, num);
		}

		public static bool HasRegionCode(string code)
		{
			return code.IndexOf('-') >= 0;
		}

		public static string GetRegionCode(string code)
		{
			var num = code.IndexOf('-');
			if (num < 0) return string.Empty;
			return code.Substring(num + 1);
		}

		public static string MakeFullCode(string langCode, string regionCode)
		{
			return $"{langCode}-{regionCode}";
		}

		public static bool AreCodesEqual(string codeA, string codeB)
		{
			return string.Equals(codeA, codeB, StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool IsValidLanguage(string language)
		{
			var name = language;
			if (IsDeveloperLanguage(language)) name = GetLanguageCode(language);
			try
			{
				CultureInfo.GetCultureInfo(name);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		public static bool IsDeveloperLanguage(string language)
		{
			if (HasRegionCode(language)) return AreCodesEqual(GetRegionCode(language), DeveloperRegionCode);
			return false;
		}

		public static bool IsValidModuleName(string moduleName)
		{
			if (moduleName.Length == 0) return false;
			return moduleName.IndexOfAny(InvalidModuleNameCharacters) < 0;
		}

		public static int ParseInt(string value, int undefinedValue)
		{
			var result = default(int);
			if (int.TryParse(value, out result)) return result;
			return undefinedValue;
		}

		public static T ParseEnum<T>(string value, T undefinedValue)
		{
			var typeFromHandle = typeof(T);
			var val = undefinedValue;
			if (!string.IsNullOrEmpty(value))
				try
				{
					val = (T) Enum.Parse(typeFromHandle, value);
					if (!Enum.IsDefined(typeFromHandle, val))
					{
						val = undefinedValue;
						return val;
					}

					return val;
				}
				catch (ArgumentException)
				{
					return val;
				}
				catch (InvalidCastException)
				{
					return val;
				}

			return val;
		}

		public static void SetDefaultNamespace(this XElement element, XNamespace newXmlns)
		{
			var currentXmlns = element.GetDefaultNamespace();
			if (!(currentXmlns == newXmlns))
				foreach (var item in from e in element.DescendantsAndSelf()
					where e.Name.Namespace == currentXmlns
					select e)
					item.Name = newXmlns.GetName(item.Name.LocalName);
		}
	}
}