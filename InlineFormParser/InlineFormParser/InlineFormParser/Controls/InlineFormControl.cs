using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using InlineFormParser.Model;

namespace InlineFormParser.Controls
{
	
public class InlineFormControl : UserControlBase, IHasUserInputs, IComponentConnector, IStyleConnector
{
	private TextBoxUpdater updater;

	internal InlineFormControl root;

	private bool _contentLoaded;

	public bool HasKeyboardAdapter => KeyboardAdapter != null;

	public InlineFormBase InlineForm => (InlineFormBase)DataContext;

	public FieldPopupKeyboardAdapter KeyboardAdapter { get; set; }

	public InlineFormControl(InlineFormBase ilf, int desiredWidth)
	{
		Resize(desiredWidth);
		DataContext = ilf;
		InitializeComponent();
		updater = new TextBoxUpdater(this);
	}

	public void Resize(int newWidth)
	{
		if (newWidth < 40)
		{
			throw new ArgumentException("newWidth is too small");
		}
		Width = (double)newWidth;
	}

	public void Update(InlineFormBase ilf)
	{
		DataContext = ilf;
		updater = new TextBoxUpdater(this);
	}

	public void UpdatePendingInputs()
	{
		updater.UpdateTextBoxesToSource(false);
	}

	private void OnButtonRequestParamListClick(object sender, RoutedEventArgs e)
	{
		ParamListField paramListField = (ParamListField)((Control)sender).DataContext;
		paramListField.RequestParameterList();
	}

	private void OnFieldPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		UpdatePendingInputs();
		Field field = (Field)((FrameworkElement)sender).DataContext;
		field.Select();
	}

	private void OnListFieldMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (KeyboardAdapter != null)
		{
			FrameworkElement frameworkElement = (FrameworkElement)sender;
			ListField listField = (ListField)frameworkElement.DataContext;
			IPopupManager popupManager = KeyboardAdapter.PopupManager;
			if (popupManager.OpenPopupCallerData == listField)
			{
				popupManager.CloseAnyPopup();
			}
			else
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["Items"] = listField.DisplayedItems;
				dictionary["SelectedIndex"] = listField.CurrentSelectedIndex;
				popupManager.OpenPopup("ListSelectorPopup", new WPFInputControl(frameworkElement), dictionary, OnListSelectorPopupEvent, listField);
			}
		}
	}

	private void OnListSelectorPopupEvent(PopupEvent popupEvent, object callerData, object result)
	{
		if (popupEvent == PopupEvent.Closed)
		{
			ListField listField = (ListField)callerData;
			int num = (int)result;
			if (num >= 0)
			{
				listField.CurrentSelectedIndex = num;
			}
		}
	}

	private void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
	{
		Field field = (Field)((FrameworkElement)sender).DataContext;
		field.Select();
	}

	private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (KeyboardAdapter != null)
		{
			KeyboardAdapter.ProcessTextBoxKey((TextBox)sender, e);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/KukaRoboter.Common.KfdXml;component/inlineforms/inlineformcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			root = (InlineFormControl)target;
		}
		else
		{
			_contentLoaded = true;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 2:
		{
			EventSetter eventSetter = new EventSetter();
			eventSetter.Event = GotFocusEvent;
			eventSetter.Handler = new RoutedEventHandler(OnTextBoxGotFocus);
			((Style)target).Setters.Add(eventSetter);
			break;
		}
		case 3:
		{
			EventSetter eventSetter = new EventSetter();
			eventSetter.Event = GotFocusEvent;
			eventSetter.Handler = new RoutedEventHandler(OnTextBoxGotFocus);
			((Style)target).Setters.Add(eventSetter);
			break;
		}
		case 4:
			((InputValidateTextBox)target).PreviewKeyDown += this.OnTextBoxPreviewKeyDown;
			break;
		case 5:
			((InputValidateTextBox)target).PreviewKeyDown += this.OnTextBoxPreviewKeyDown;
			break;
		case 6:
			((DelayedButton)target).Click += this.OnButtonRequestParamListClick;
			break;
		case 7:
			((InputValidateTextBox)target).PreviewKeyDown += this.OnTextBoxPreviewKeyDown;
			break;
		case 8:
			((Border)target).MouseLeftButtonDown += OnListFieldMouseLeftButtonDown;
			break;
		case 9:
			((DockPanel)target).PreviewMouseLeftButtonDown += OnFieldPreviewMouseLeftButtonDown;
			break;
		}
	}
}
}