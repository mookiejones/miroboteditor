#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:45 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Common.ResourceAccess
{
	public interface IIndexedResourceAccessor : IResourceAccessor
	{
		IndexedAccess<string> Strings { get; }

		IndexedAccess<object> Objects { get; }
	}
}