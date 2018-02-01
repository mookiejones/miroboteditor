namespace InlineFormParser.Model
{
	public interface IViewConfiguration
	{
		string ViewSystemName
		{
			get;
		}

		string ViewTypeName
		{
			get;
		}

		string ViewModelComponent
		{
			get;
		}

		string DisplayProvider
		{
			get;
		}

		string DisplayID
		{
			get;
		}

		bool OpenOnStartup
		{
			get;
		}

		string Caption
		{
			get;
		}

		string ConnectionArgument
		{
			get;
		}
	}
}