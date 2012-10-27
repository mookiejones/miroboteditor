/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 9/23/2012
 * Time: 9:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace miRobotEditor
{
	
	public interface ICompletionItem
	{
		string Text { get; }
		string Description { get; }
		IImage Image { get; }
		
		/// <summary>
		/// Performs code completion for the item.
		/// </summary>
		void Complete(CompletionContext context);
		
		/// <summary>
		/// Gets a priority value for the completion data item.
		/// When selecting items by their start characters, the item with the highest
		/// priority is selected first.
		/// </summary>
		double Priority {
			get;
		}
	}
	
	/// <summary>
	/// Completion item that supports complex content and description.
	/// </summary>
	public interface IFancyCompletionItem : ICompletionItem
	{
		object Content { get; }
		new object Description { get; }
	}
	
	public interface ISnippetCompletionItem : ICompletionItem
	{
		string Keyword { get; }
	}
	
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
