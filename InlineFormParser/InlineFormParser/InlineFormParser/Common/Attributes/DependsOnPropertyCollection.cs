#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:28 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.Collections.Generic;

#endregion

namespace InlineFormParser.Common.Attributes
{
	internal class DependsOnPropertyCollection : List<string>
	{
		internal DependsOnPropertyCollection(string masterPropertyName)
		{
			MasterPropertyName = masterPropertyName;
		}

		internal string MasterPropertyName { get; }
	}
}