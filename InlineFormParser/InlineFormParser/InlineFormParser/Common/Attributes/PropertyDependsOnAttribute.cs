#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:29 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class PropertyDependsOnAttribute : Attribute
	{
		public PropertyDependsOnAttribute(params string[] dependsOnPropertyNames)
		{
			if (dependsOnPropertyNames == null || dependsOnPropertyNames.Length == 0)
				throw new ArgumentException("PropertyDependsOnAttribute requires at least one argument.");


			for (var i = 0; i < dependsOnPropertyNames.Length; i++)
				if (string.IsNullOrEmpty(dependsOnPropertyNames[i]))
					throw new ArgumentNullException($"dependsOnPropertyNames[{i}]");
			DependsOnPropertyNames = dependsOnPropertyNames;
		}

		public string[] DependsOnPropertyNames { get; }
	}
}