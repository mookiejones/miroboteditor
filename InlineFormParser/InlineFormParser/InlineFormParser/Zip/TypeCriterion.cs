#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:20 AM
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
	internal class TypeCriterion : SelectionCriterion
	{
		private char objectType;
		internal ComparisonOperator Operator;

		internal string AttributeString
		{
			get => objectType.ToString(CultureInfo.InvariantCulture);
			set
			{
				if (value.Length == 1 && (value[0] == 'D' || value[0] == 'F'))
				{
					objectType = value[0];
					return;
				}

				throw new ArgumentException("Specify a single character: either D or F");
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("type ").Append(EnumUtil.GetDescription(Operator)).Append(" ")
				.Append(AttributeString);
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			bool flag = objectType == 'D' ? Directory.Exists(filename) : File.Exists(filename);
			if (Operator != ComparisonOperator.EqualTo) flag = !flag;
			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			bool flag = objectType == 'D' ? entry.IsDirectory : !entry.IsDirectory;
			if (Operator != ComparisonOperator.EqualTo) flag = !flag;
			return flag;
		}
	}
}