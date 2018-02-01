#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:23 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Reflection;
using System.Resources;
using Component = InlineFormParser.Model.AdeComponent;
#endregion

namespace InlineFormParser.Model
{
	public class ComponentResourceAccessor : CommonResourceAccessor
	{
		private readonly ResourceManager[] managers;

		public ComponentResourceAccessor(Component component)
			: this(component, true)
		{
			if (component != null) managers = GetAssemblyResourceManagers(component.GetType().Assembly);
		}

		public ComponentResourceAccessor(Component component, bool observeCultureChanges)
			: base(component?.CommonResourceService, observeCultureChanges)
		{
			if (component != null) managers = GetAssemblyResourceManagers(component.GetType().Assembly);
		}

		public ComponentResourceAccessor(ICommonResourceService2 commonResourceService)
			: base(commonResourceService)
		{
			managers = null;
		}

		public ComponentResourceAccessor(ICommonResourceService2 commonResourceService, Assembly assembly)
			: base(commonResourceService)
		{
			if (assembly == null) throw new ArgumentNullException(nameof(assembly));
			managers = GetAssemblyResourceManagers(assembly);
		}

		protected override string GetString(string key)
		{
			if (managers != null && key.IndexOf('@') < 0)
			{
				string stringFromResourceManagers = GetStringFromResourceManagers(managers, key);
				if (stringFromResourceManagers != null) return stringFromResourceManagers;
			}

			return base.GetString(key);
		}

		protected override object GetObject(string key)
		{
			if (managers != null && key.IndexOf('@') < 0)
			{
				object objectFromResourceManagers = GetObjectFromResourceManagers(managers, key);
				if (objectFromResourceManagers != null) return objectFromResourceManagers;
			}

			return base.GetObject(key);
		}
	}
}