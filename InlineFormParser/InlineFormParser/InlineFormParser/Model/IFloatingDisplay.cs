namespace InlineFormParser.Model
{
	public interface IFloatingDisplay
	{
		ClosingKeys CloseOnKeyMode
		{
			get;
			set;
		}

		FloatingDisplayParameters Parameters
		{
			get;
			set;
		}

		void UpdateLayout(bool tryKeepPosition);
	}
}