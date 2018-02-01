#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:24 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using InlineFormParser.Common.Attributes;

#endregion

namespace InlineFormParser.Model
{
	public class CommonResourceAccessor : ResourceAccessorBase
	{
		private readonly ICommonResourceService2 commonResourceService;

		public CommonResourceAccessor([CanBeNull] ICommonResourceService2 commonResourceService)
			: this(commonResourceService, true)
		{
		}

		public CommonResourceAccessor([CanBeNull] ICommonResourceService2 commonResourceService, bool observeCultureChanges)
			: base(observeCultureChanges)
		{
			this.commonResourceService = commonResourceService;
		}

		protected override string GetString(string key)
		{
			string text = commonResourceService != null
				? commonResourceService.GetString(key, false, CultureSwitch.CurrentCulture)
				: null;
			if (text == null)
			{
				WarnNotFound(key);
				text = key;
			}

			return text;
		}

		protected override object GetObject(string key)
		{
			object obj = null;
			if (commonResourceService != null)
			{
				obj = commonResourceService.GetImage(key, false, CultureSwitch.CurrentCulture);
				if (obj == null) obj = commonResourceService.GetIcon(key, false, CultureSwitch.CurrentCulture);
			}

			WarnNotFound(key);
			return obj;
		}
	}
}