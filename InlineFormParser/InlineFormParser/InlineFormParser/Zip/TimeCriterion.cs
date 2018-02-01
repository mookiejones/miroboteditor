﻿#region Project Info

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
using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	internal class TimeCriterion : SelectionCriterion
	{
		internal ComparisonOperator Operator;

		internal DateTime Time;

		internal WhichTime Which;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Which).Append(" ").Append(EnumUtil.GetDescription(Operator))
				.Append(" ")
				.Append(Time.ToString("yyyy-MM-dd-HH:mm:ss"));
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			DateTime x;
			switch (Which)
			{
				case WhichTime.atime:
					x = File.GetLastAccessTime(filename).ToUniversalTime();
					break;
				case WhichTime.mtime:
					x = File.GetLastWriteTime(filename).ToUniversalTime();
					break;
				case WhichTime.ctime:
					x = File.GetCreationTime(filename).ToUniversalTime();
					break;
				default:
					throw new ArgumentException("Operator");
			}

			return _Evaluate(x);
		}

		private bool _Evaluate(DateTime x)
		{
			switch (Operator)
			{
				case ComparisonOperator.GreaterThanOrEqualTo:
					return x >= Time;
				case ComparisonOperator.GreaterThan:
					return x > Time;
				case ComparisonOperator.LesserThanOrEqualTo:
					return x <= Time;
				case ComparisonOperator.LesserThan:
					return x < Time;
				case ComparisonOperator.EqualTo:
					return x == Time;
				case ComparisonOperator.NotEqualTo:
					return x != Time;
				default:
					throw new ArgumentException("Operator");
			}
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			DateTime x;
			switch (Which)
			{
				case WhichTime.atime:
					x = entry.AccessedTime;
					break;
				case WhichTime.mtime:
					x = entry.ModifiedTime;
					break;
				case WhichTime.ctime:
					x = entry.CreationTime;
					break;
				default:
					throw new ArgumentException("??time");
			}

			return _Evaluate(x);
		}
	}
}