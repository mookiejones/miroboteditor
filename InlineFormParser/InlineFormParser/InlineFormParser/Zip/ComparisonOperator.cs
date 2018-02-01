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

using System.ComponentModel;

#endregion

namespace InlineFormParser.Zip
{
	internal enum ComparisonOperator
	{
		[Description(">")] GreaterThan,
		[Description(">=")] GreaterThanOrEqualTo,
		[Description("<")] LesserThan,
		[Description("<=")] LesserThanOrEqualTo,
		[Description("=")] EqualTo,
		[Description("!=")] NotEqualTo
	}
}