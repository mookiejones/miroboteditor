#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:22 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace InlineFormParser.Zip
{
	internal class NameCriterion : SelectionCriterion
	{
		private string matchingFileSpec;
		internal ComparisonOperator Operator;

		private Regex re;

		private string regexString;

		internal virtual string MatchingFileSpec
		{
			set
			{
				if (Directory.Exists(value))
					matchingFileSpec = $".\\{value}\\*.*";
				else
					matchingFileSpec = value;
				regexString =
					$"^{Regex.Escape(matchingFileSpec).Replace("\\\\\\*\\.\\*", "\\\\([^\\.]+|.*\\.[^\\\\\\.]*)").Replace("\\.\\*", "\\.[^\\\\\\.]*").Replace("\\*", ".*").Replace("\\?", "[^\\\\\\.]")}$";
				re = new Regex(regexString, RegexOptions.IgnoreCase);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("name ").Append(EnumUtil.GetDescription(Operator)).Append(" '")
				.Append(matchingFileSpec)
				.Append("'");
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			return _Evaluate(filename);
		}

		private bool _Evaluate(string fullpath)
		{
			string text = matchingFileSpec.IndexOf('\\') == -1 ? Path.GetFileName(fullpath) : fullpath;
			bool flag = text != null && re.IsMatch(text);
			if (Operator != ComparisonOperator.EqualTo) flag = !flag;
			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			string fullpath = entry.FileName.Replace("/", "\\");
			return _Evaluate(fullpath);
		}
	}
}