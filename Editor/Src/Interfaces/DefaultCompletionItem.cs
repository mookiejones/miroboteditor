namespace miRobotEditor.Interfaces
{
    public class DefaultCompletionItem : ICompletionItem
    {
        public string Text { get; private set; }
        public virtual string Description { get; set; }
        public virtual IImage Image { get; set; }
		
        public virtual double Priority { get { return 0; } }
		
        public DefaultCompletionItem(string text)
        {
            this.Text = text;
        }
		
        public virtual void Complete(CompletionContext context)
        {
            context.Editor.Document.Replace(context.StartOffset, context.Length, this.Text);
            context.EndOffset = context.StartOffset + this.Text.Length;
        }
    }
}