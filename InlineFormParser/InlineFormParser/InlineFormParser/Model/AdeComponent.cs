using System.Collections;
using System.ComponentModel;
using InlineFormParser.Common.Attributes;

namespace InlineFormParser.Model
{

	[DesignerCategory("Component")]
	[AdeComponent]
	public class AdeComponent : Component, IAdeCommandProvider
	{
		private ICommonResourceService2 commonResourceService;

		private IAdeConfigurationService configurationService;

		public override ISite Site
		{
			get => base.Site;
			set
			{
				base.Site = value;
				if (value != null)
				{
					ComponentFramework = (value.GetService(typeof(IAdeComponentFramework)) as IAdeComponentFramework);
					if (ComponentFramework != null)
					{
						Initialize();
					}
				}
			}
		}

		public IAdeComponentFramework ComponentFramework { get; private set; }

		public virtual IEnumerable Commands => new Command[0];

		public ICommonResourceService2 CommonResourceService
		{
			get
			{
				if (commonResourceService == null)
				{
					commonResourceService = (ICommonResourceService2)GetService(typeof(ICommonResourceService2));
				}
				return commonResourceService;
			}
		}

		public IAdeConfigurationService ConfigurationService
		{
			get
			{
				if (configurationService == null)
				{
					configurationService = (IAdeConfigurationService)GetService(typeof(IAdeConfigurationService));
				}
				return configurationService;
			}
		}

		public virtual void Initialize()
		{
		}
	}
}