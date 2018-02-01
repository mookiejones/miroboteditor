namespace InlineFormParser.Model
{
	public interface IDialogHandler
	{
		bool IsActive
		{
			get;
		}

		void Close(object answerData);
	}
}