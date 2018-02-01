#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:47 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Common.ResourceAccess
{
	public interface IResourceAccessor
	{
		object GetObject(object requester, string key);

		string GetString(object requester, string key, params object[] args);
	}
}