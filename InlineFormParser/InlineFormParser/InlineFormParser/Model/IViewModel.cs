namespace InlineFormParser.Model
{
	public interface IViewModel
	{
		bool IsConnected
		{
			get;
		}

		void Connect();

		void Disconnect();
	}
}