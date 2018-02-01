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

using System;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	internal class SizeCriterion : SelectionCriterion
	{
		internal ComparisonOperator Operator;

		internal long Size;

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append("size ").Append(EnumUtil.GetDescription(Operator)).Append(" ")
				.Append(Size.ToString(CultureInfo.InvariantCulture));
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			var fileInfo = new FileInfo(filename);
			return _Evaluate(fileInfo.Length);
		}

		private bool _Evaluate(long Length)
		{
			switch (Operator)
			{
				case ComparisonOperator.GreaterThanOrEqualTo:
					return Length >= Size;
				case ComparisonOperator.GreaterThan:
					return Length > Size;
				case ComparisonOperator.LesserThanOrEqualTo:
					return Length <= Size;
				case ComparisonOperator.LesserThan:
					return Length < Size;
				case ComparisonOperator.EqualTo:
					return Length == Size;
				case ComparisonOperator.NotEqualTo:
					return Length != Size;
				default:
					throw new ArgumentException("Operator");
			}
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			return _Evaluate(entry.UncompressedSize);
		}
	}
}