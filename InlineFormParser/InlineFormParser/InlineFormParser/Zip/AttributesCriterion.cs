#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:21 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	internal class AttributesCriterion : SelectionCriterion
	{
		private FileAttributes attributes;
		internal ComparisonOperator Operator;

		internal string AttributeString
		{
			get
			{
				string text = string.Empty;
				if ((attributes & FileAttributes.Hidden) != 0) text += "H";
				if ((attributes & FileAttributes.System) != 0) text += "S";
				if ((attributes & FileAttributes.ReadOnly) != 0) text += "R";
				if ((attributes & FileAttributes.Archive) != 0) text += "A";
				if ((attributes & FileAttributes.ReparsePoint) != 0) text += "L";
				if ((attributes & FileAttributes.NotContentIndexed) != 0) text += "I";
				return text;
			}
			set
			{
				attributes = FileAttributes.Normal;
				string text = value.ToUpper();
				foreach (char c in text)
					switch (c)
					{
						case 'H':
							if ((attributes & FileAttributes.Hidden) != 0)
								throw new ArgumentException($"Repeated flag. ({c})", nameof(value));
							attributes |= FileAttributes.Hidden;
							break;
						case 'R':
							if ((attributes & FileAttributes.ReadOnly) != 0)
								throw new ArgumentException($"Repeated flag. ({c})", nameof(value));
							attributes |= FileAttributes.ReadOnly;
							break;
						case 'S':
							if ((attributes & FileAttributes.System) != 0)
								throw new ArgumentException($"Repeated flag. ({c})", nameof(value));
							attributes |= FileAttributes.System;
							break;
						case 'A':
							if ((attributes & FileAttributes.Archive) != 0)
								throw new ArgumentException($"Repeated flag. ({c})", nameof(value));
							attributes |= FileAttributes.Archive;
							break;
						case 'I':
							if ((attributes & FileAttributes.NotContentIndexed) != 0)
								throw new ArgumentException($"Repeated flag. ({c})", nameof(value));
							attributes |= FileAttributes.NotContentIndexed;
							break;
						case 'L':
							if ((attributes & FileAttributes.ReparsePoint) != 0)
								throw new ArgumentException($"Repeated flag. ({c})", nameof(value));
							attributes |= FileAttributes.ReparsePoint;
							break;
						default:
							throw new ArgumentException(value);
					}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("attributes ").Append(EnumUtil.GetDescription(Operator)).Append(" ")
				.Append(AttributeString);
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			if (Directory.Exists(filename)) return Operator != ComparisonOperator.EqualTo;
			FileAttributes fileAttrs = File.GetAttributes(filename);
			return _Evaluate(fileAttrs);
		}

		private bool _EvaluateOne(FileAttributes fileAttrs, FileAttributes criterionAttrs)
		{
			if ((attributes & criterionAttrs) == criterionAttrs) return (fileAttrs & criterionAttrs) == criterionAttrs;
			return true;
		}

		private bool _Evaluate(FileAttributes fileAttrs)
		{
			bool flag = _EvaluateOne(fileAttrs, FileAttributes.Hidden);
			if (flag) flag = _EvaluateOne(fileAttrs, FileAttributes.System);
			if (flag) flag = _EvaluateOne(fileAttrs, FileAttributes.ReadOnly);
			if (flag) flag = _EvaluateOne(fileAttrs, FileAttributes.Archive);
			if (flag) flag = _EvaluateOne(fileAttrs, FileAttributes.NotContentIndexed);
			if (flag) flag = _EvaluateOne(fileAttrs, FileAttributes.ReparsePoint);
			if (Operator != ComparisonOperator.EqualTo) flag = !flag;
			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			FileAttributes fileAttrs = entry.Attributes;
			return _Evaluate(fileAttrs);
		}
	}
}