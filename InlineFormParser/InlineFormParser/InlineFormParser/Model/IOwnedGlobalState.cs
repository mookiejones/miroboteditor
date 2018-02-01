namespace InlineFormParser.Model
{
	public interface IOwnedGlobalState
	{
		IGlobalState State
		{
			get;
		}

		void SetValue(object value);
	}
}