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

using System;
using System.Collections.Generic;

#endregion

namespace InlineFormParser.Common.Attributes
{
	internal class AutoNotifyPropertyDictionary : Dictionary<string, DependsOnPropertyCollection>
	{
		internal AutoNotifyPropertyDictionary(Type senderType)
		{
			SenderType = senderType;
		}

		internal Type SenderType { get; }

		internal bool CheckCyclicDependency(string masterPropertyName, string dependentPropertyName)
		{
			var checkedProperties = new List<string>();
			return RecCheckCyclicDependency(masterPropertyName, dependentPropertyName, checkedProperties);
		}

		private bool RecCheckCyclicDependency(string masterPropertyName, string dependentPropertyName,
			List<string> checkedProperties)
		{
			checkedProperties.Add(dependentPropertyName);
			if (masterPropertyName == dependentPropertyName) return true;
			if (!ContainsKey(dependentPropertyName)) return false;
			var dependsOnPropertyCollection = base[dependentPropertyName];
			foreach (var item in dependsOnPropertyCollection)
				if (!checkedProperties.Contains(item) && RecCheckCyclicDependency(masterPropertyName, item, checkedProperties))
					return true;
			return false;
		}
	}
}