using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace InlineFormParser.Controls
{
	
	public class TextBoxUpdater : DependencyObject
	{
		private UIElement root;

		private List<BindingExpression> bindings;

		public TextBoxUpdater(UIElement root)
			: this(root, false)
		{
		}

		public TextBoxUpdater(UIElement root, bool gatherNow)
		{
			this.root = root ?? throw new ArgumentNullException(nameof(root));
			if (gatherNow)
			{
				this.GatherBindings();
			}
		}

		public void UpdateTextBoxesToSource(bool rebuild)
		{
			base.VerifyAccess();
			if (rebuild)
			{
				this.bindings = null;
			}
			if (this.bindings == null)
			{
				this.GatherBindings();
			}
			foreach (BindingExpression binding in this.bindings)
			{
				binding.UpdateSource();
			}
		}

		public void FreeBindings()
		{
			this.bindings = null;
		}

		private void GatherBindings()
		{
			base.VerifyAccess();
			this.bindings = new List<BindingExpression>();
			this.RecAddBindings(this.root);
		}

		private void RecAddBindings(DependencyObject node)
		{
			TextBox textBox = node as TextBox;
			if (textBox != null)
			{
				BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
				if (bindingExpression != null)
				{
					this.bindings.Add(bindingExpression);
				}
			}
			int childrenCount = VisualTreeHelper.GetChildrenCount(node);
			for (int i = 0; i < childrenCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(node, i);
				this.RecAddBindings(child);
			}
		}
	}
}
