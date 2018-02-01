using System;
using System.Windows;

namespace InlineFormParser.Model
{
	public abstract class LogicalDisplay : IContentFrame
	{
		public abstract string ID
		{
			get;
		}

		public virtual bool IsPermanent => true;

		public virtual bool IsOpen => true;

		public virtual bool HasCaption => false;

		public virtual string Caption
		{
			get => throw new InvalidOperationException($"Caption for display \"{ID}\" can not be requested.");
			set
			{
				throw new InvalidOperationException($"Caption for display \"{ID}\" can not be set.");
			}
		}

		public virtual bool IsVisibleWhenOpen => true;

		public abstract UIElement Content
		{
			get;
			set;
		}

		public virtual void Open(object displayParameters)
		{
			if (!IsPermanent)
			{
				return;
			}
			throw new InvalidOperationException($"Permanent display \"{ID}\" cannot be opened!");
		}

		public virtual void Close()
		{
			if (!IsPermanent)
			{
				return;
			}
			throw new InvalidOperationException($"Permanent display \"{ID}\" cannot be closed!");
		}

		public virtual void Select()
		{
		}
	}
}