namespace InlineFormParser.Model
{
	public class DialogAnswer
	{
		public object[] Args
		{
			get;
			private set;
		}

		public string Key
		{
			get;
			private set;
		}

		public string AnswerResourceKey
		{
			get;
			private set;
		}

		public DialogAnswer(string key, string answer, params object[] args)
		{
			Key = key;
			Args = args;
			AnswerResourceKey = answer;
		}
	}

}