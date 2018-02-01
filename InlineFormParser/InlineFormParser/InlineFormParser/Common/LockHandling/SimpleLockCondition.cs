#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:44 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using InlineFormParser.Common.ResourceAccess;

#endregion

namespace InlineFormParser.Common.LockHandling
{
	public class SimpleLockCondition : LockCondition
	{
		private readonly Predicate<object> isLocked;

		private readonly IIndexedResourceAccessor resourceAccessor;

		private readonly string resourceKey;

		public SimpleLockCondition(Predicate<object> isLocked, IIndexedResourceAccessor resourceAccessor, string resourceKey)
		{
			this.resourceKey = resourceKey ?? string.Empty;
			this.isLocked = isLocked;
			this.resourceAccessor = resourceAccessor;
		}

		public override bool IsConditionFullfilled => isLocked == null || isLocked(null);

		public override void GetLockInfo(out IEnumerable<string> result)
		{
			result = new List<string>();
			if (!IsConditionFullfilled && !result.Contains(resourceKey) && resourceAccessor.Strings[resourceKey] != string.Empty)
				(result as List<string>).Add(resourceAccessor.Strings[resourceKey]);
		}
	}
}