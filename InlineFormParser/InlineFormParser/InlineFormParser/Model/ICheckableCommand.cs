namespace InlineFormParser.Model
{
	public interface ICheckableCommand
	{
		void Check(object context, object[] parameters);
	}
}