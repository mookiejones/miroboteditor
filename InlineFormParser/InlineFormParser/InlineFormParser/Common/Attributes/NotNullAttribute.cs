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

#endregion

namespace InlineFormParser.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field |
	                AttributeTargets.Parameter | AttributeTargets.Delegate)]
	public sealed class NotNullAttribute : Attribute
	{
	}
}