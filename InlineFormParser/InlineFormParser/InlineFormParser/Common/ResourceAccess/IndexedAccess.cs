#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:46 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Common.ResourceAccess
{
	public class IndexedAccess<T>
	{
		public delegate T AccessDelegate(string key);

		private readonly AccessDelegate accessor;

		public IndexedAccess(AccessDelegate accessor)
		{
			this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
		}

		public T this[string key] => accessor(key);
	}
}