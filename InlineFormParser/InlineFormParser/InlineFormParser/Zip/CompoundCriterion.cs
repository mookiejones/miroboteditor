#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:19 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	internal class CompoundCriterion : SelectionCriterion
	{
		internal LogicalConjunction Conjunction;

		internal SelectionCriterion Left;

		private SelectionCriterion right;

		internal SelectionCriterion Right
		{
			get => right;
			set
			{
				right = value;
				if (value == null)
					Conjunction = LogicalConjunction.NONE;
				else if (Conjunction == LogicalConjunction.NONE)
					Conjunction = LogicalConjunction.AND;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(").Append(Left != null ? Left.ToString() : "null").Append(" ")
				.Append(Conjunction)
				.Append(" ")
				.Append(Right != null ? Right.ToString() : "null")
				.Append(")");
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			bool flag = Left.Evaluate(filename);
			switch (Conjunction)
			{
				case LogicalConjunction.AND:
					if (flag) flag = Right.Evaluate(filename);
					break;
				case LogicalConjunction.OR:
					if (!flag) flag = Right.Evaluate(filename);
					break;
				case LogicalConjunction.XOR:
					flag ^= Right.Evaluate(filename);
					break;
				default:
					throw new ArgumentException("Conjunction");
			}

			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			bool flag = Left.Evaluate(entry);
			switch (Conjunction)
			{
				case LogicalConjunction.AND:
					if (flag) flag = Right.Evaluate(entry);
					break;
				case LogicalConjunction.OR:
					if (!flag) flag = Right.Evaluate(entry);
					break;
				case LogicalConjunction.XOR:
					flag ^= Right.Evaluate(entry);
					break;
			}

			return flag;
		}
	}
}