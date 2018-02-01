using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using InlineFormParser.Common.LockHandling;

namespace InlineFormParser.Model
{

public static class LockManager
{
	public static readonly DependencyProperty IsLockedProperty = DependencyProperty.RegisterAttached("IsLocked", typeof(bool), typeof(LockManager), new FrameworkPropertyMetadata(false, OnIsLockedChanged)
	{
		DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
	});

	public static readonly DependencyProperty LockAdornerSiteForProperty = DependencyProperty.RegisterAttached("LockAdornerSiteFor", typeof(DependencyObject), typeof(LockManager), new FrameworkPropertyMetadata(null, OnLockAdornerSiteForChanged));

	public static readonly DependencyProperty LockAdornerSiteProperty = DependencyProperty.RegisterAttached("LockAdornerSite", typeof(DependencyObject), typeof(LockManager), new FrameworkPropertyMetadata(null, OnLockAdornerSiteChanged));

	public static readonly RoutedEvent LockChangedEvent = EventManager.RegisterRoutedEvent("LockChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(LockManager));

	public static readonly DependencyProperty LockConditionProperty = DependencyProperty.RegisterAttached("LockCondition", typeof(ILockCondition), typeof(LockManager), new FrameworkPropertyMetadata(null, OnLockConditionChanged)
	{
		DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
	});

	public static readonly DependencyProperty LockTemplateProperty = DependencyProperty.RegisterAttached("LockTemplate", typeof(ControlTemplate), typeof(LockManager), new FrameworkPropertyMetadata(CreateDefaultLockTemplate(), FrameworkPropertyMetadataOptions.NotDataBindable, OnLockTemplateChanged));

	private static readonly DependencyProperty LockAdornerProperty = DependencyProperty.RegisterAttached("LockAdorner", typeof(TemplatedAdorner), typeof(LockManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable));

	private static ControlTemplate defaultTemplate;

	public static void AddLockChangedHandler(DependencyObject element, RoutedPropertyChangedEventHandler<bool> handler)
	{
		var uIElement = element as UIElement;
		if (uIElement != null)
		{
			uIElement.AddHandler(LockChangedEvent, handler);
		}
	}

	[AttachedPropertyBrowsableForType(typeof(UIElement))]
	public static bool GetIsLocked(DependencyObject element)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		return (bool)element.GetValue(IsLockedProperty);
	}

	[AttachedPropertyBrowsableForType(typeof(Control))]
	public static DependencyObject GetLockAdornerSite(DependencyObject element)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		return element.GetValue(LockAdornerSiteProperty) as DependencyObject;
	}

	[AttachedPropertyBrowsableForType(typeof(Control))]
	public static DependencyObject GetLockAdornerSiteFor(DependencyObject element)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		return element.GetValue(LockAdornerSiteForProperty) as DependencyObject;
	}

	[AttachedPropertyBrowsableForType(typeof(UIElement))]
	public static ILockCondition GetLockCondition(DependencyObject element)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		return (ILockCondition)element.GetValue(LockConditionProperty);
	}

	[AttachedPropertyBrowsableForType(typeof(Control))]
	public static ControlTemplate GetLockTemplate(DependencyObject element)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		return element.GetValue(LockTemplateProperty) as ControlTemplate;
	}

	public static void RemoveLockChangedHandler(DependencyObject element, RoutedPropertyChangedEventHandler<bool> handler)
	{
		var uIElement = element as UIElement;
		if (uIElement != null)
		{
			uIElement.RemoveHandler(LockChangedEvent, handler);
		}
	}

	[AttachedPropertyBrowsableForType(typeof(UIElement))]
	public static void SetIsLocked(DependencyObject element, bool value)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		var objA = element.ReadLocalValue(IsLockedProperty);
		if (!Equals(objA, value))
		{
			element.SetValue(IsLockedProperty, value);
		}
	}

	[AttachedPropertyBrowsableForType(typeof(Control))]
	public static void SetLockAdornerSite(DependencyObject element, DependencyObject value)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		element.SetValue(LockAdornerSiteProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(Control))]
	public static void SetLockAdornerSiteFor(DependencyObject element, DependencyObject value)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		element.SetValue(LockAdornerSiteForProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(UIElement))]
	public static void SetLockCondition(DependencyObject element, ILockCondition value)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		var objA = element.ReadLocalValue(LockConditionProperty);
		if (!Equals(objA, value))
		{
			element.SetValue(LockConditionProperty, value);
		}
	}

	[AttachedPropertyBrowsableForType(typeof(Control))]
	public static void SetLockTemplate(DependencyObject element, ControlTemplate value)
	{
		if (element == null)
		{
			throw new ArgumentNullException(nameof(element));
		}
		var objA = element.ReadLocalValue(LockTemplateProperty);
		if (!Equals(objA, value))
		{
			element.SetValue(LockTemplateProperty, value);
		}
	}

	private static ControlTemplate CreateDefaultLockTemplate()
	{
		if (defaultTemplate != null)
		{
			return defaultTemplate;
		}
		defaultTemplate = new ControlTemplate(typeof(Control));
		var frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel), "PART_LockPanel");
		var frameworkElementFactory2 = new FrameworkElementFactory(typeof(Image), "PART_LockImage");
		frameworkElementFactory2.SetValue(Image.SourceProperty, LoadLockImage());
		frameworkElementFactory2.SetValue(FrameworkElement.WidthProperty, 16.0);
		frameworkElementFactory2.SetValue(FrameworkElement.HeightProperty, 16.0);
		frameworkElementFactory2.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
		frameworkElementFactory2.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right);
		frameworkElementFactory2.SetValue(FrameworkElement.MarginProperty, new Thickness(0.0, 2.0, 0.0, 0.0));
		var frameworkElementFactory3 = new FrameworkElementFactory(typeof(AdornedElementPlaceholderExt), "PART_PlaceHolder");
		frameworkElementFactory3.AppendChild(frameworkElementFactory2);
		frameworkElementFactory.AppendChild(frameworkElementFactory3);
		defaultTemplate.VisualTree = frameworkElementFactory;
		frameworkElementFactory.SetBinding(UIElement.VisibilityProperty, new Binding("AdornedElement.IsVisible")
		{
			ElementName = "PART_PlaceHolder",
			Converter = new BooleanToVisibilityConverter(),
			Mode = BindingMode.OneWay
		});
		defaultTemplate.Seal();
		return defaultTemplate;
	}

	private static void DisposeLockAdorner(DependencyObject targetElement)
	{
		var dependencyObject = GetLockAdornerSite(targetElement);
		if (dependencyObject == null)
		{
			dependencyObject = targetElement;
		}
		DisposeLockAdorner(targetElement, dependencyObject);
	}

	private static void DisposeLockAdorner(DependencyObject targetElement, DependencyObject adornerSite)
	{
		var uIElement = adornerSite as UIElement;
		if (uIElement != null)
		{
			var adornerLayer = AdornerLayer.GetAdornerLayer(uIElement);
			var templatedAdorner = uIElement.ReadLocalValue(LockAdornerProperty) as TemplatedAdorner;
			if (adornerLayer != null && templatedAdorner != null)
			{
				adornerLayer.Remove(templatedAdorner);
			}
			if (templatedAdorner != null)
			{
				templatedAdorner.ClearChild();
			}
			uIElement.ClearValue(LockAdornerProperty);
			uIElement.PreviewMouseDown -= OnPreviewMouseDown;
		}
	}

	private static ImageSource LoadLockImage()
	{
		var bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.UriSource = new Uri("pack://application:,,,/KukaRoboter.Common.Controls;component/Resources/Lock.png");
		bitmapImage.EndInit();
		return bitmapImage;
	}

	private static void OnIsLockedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		var control = obj as Control;
		ControlTreatments(control, e);
		if (control != null && e.NewValue is bool)
		{
			if ((bool)e.NewValue)
			{
				control.PreviewMouseDown += OnPreviewMouseDown;
				ShowLockAdorner(obj);
			}
			else
			{
				DisposeLockAdorner(obj);
				control.PreviewMouseDown -= OnPreviewMouseDown;
			}
			RaiseIsLockedChangedEvent(obj);
		}
	}

	private static void ControlTreatments(Control control, DependencyPropertyChangedEventArgs e)
	{
		if (control is TabItem && e.NewValue is bool)
		{
			if ((bool)e.NewValue)
			{
				((TabControl)control.Parent).DisableTabItemContent();
			}
			else
			{
				((TabControl)control.Parent).EnableTabItemContent();
			}
		}
	}

	private static void OnLockAdornerSiteChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		var dependencyObject = (DependencyObject)e.OldValue;
		var dependencyObject2 = (DependencyObject)e.NewValue;
		if (dependencyObject != null)
		{
			dependencyObject.ClearValue(LockAdornerSiteForProperty);
		}
		if (dependencyObject2 != null && obj != GetLockAdornerSiteFor(dependencyObject2))
		{
			SetLockAdornerSiteFor(dependencyObject2, obj);
		}
		if (GetIsLocked(obj))
		{
			if (dependencyObject == null)
			{
				dependencyObject = obj;
			}
			DisposeLockAdorner(obj, dependencyObject);
			ShowLockAdorner(obj);
		}
	}

	private static void OnLockAdornerSiteForChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		var dependencyObject = (DependencyObject)e.OldValue;
		var dependencyObject2 = (DependencyObject)e.NewValue;
		if (dependencyObject != null)
		{
			dependencyObject.ClearValue(LockAdornerSiteProperty);
		}
		if (dependencyObject2 != null && obj != GetLockAdornerSite(dependencyObject2))
		{
			SetLockAdornerSite(dependencyObject2, obj);
		}
	}

	private static void OnLockConditionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		var control = obj as Control;
		if (control != null)
		{
			var lockCondition = e.OldValue as ILockCondition;
			if (lockCondition != null)
			{
				lockCondition.RequerySuggested -= OnLockedElementRequeryTrigger;
				lockCondition.Targets.Remove(obj);
			}
			var lockCondition2 = e.NewValue as ILockCondition;
			if (lockCondition2 == null)
			{
				DisposeLockAdorner(control);
			}
			else if (!lockCondition2.Targets.Contains(obj))
			{
				lockCondition2.Targets.Add(obj);
				lockCondition2.RequerySuggested += OnLockedElementRequeryTrigger;
				SetIsLocked(obj, !lockCondition2.IsConditionFullfilled);
			}
		}
	}

	private static void OnLockTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (GetIsLocked(obj))
		{
			DisposeLockAdorner(obj);
			ShowLockAdorner(obj);
		}
	}

	private static void OnLockedElementRequeryTrigger(object sender, EventArgs e)
	{
		var lockCondition = sender as ILockCondition;
		if (lockCondition != null)
		{
			var value = !lockCondition.IsConditionFullfilled;
			foreach (var target in lockCondition.Targets)
			{
				SetIsLocked(target as DependencyObject, value);
			}
		}
	}

	private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		var dependencyObject = sender as DependencyObject;
		if (dependencyObject != null && GetIsLocked(dependencyObject))
		{
			e.Handled = true;
		}
	}

	private static void RaiseIsLockedChangedEvent(DependencyObject source)
	{
		var flag = !GetIsLocked(source);
		var routedPropertyChangedEventArgs = new RoutedPropertyChangedEventArgs<bool>(flag, !flag);
		routedPropertyChangedEventArgs.RoutedEvent = LockChangedEvent;
		var e = routedPropertyChangedEventArgs;
		if (source is ContentElement)
		{
			((ContentElement)source).RaiseEvent(e);
		}
		else if (source is UIElement)
		{
			((UIElement)source).RaiseEvent(e);
		}
		else if (source is UIElement3D)
		{
			((UIElement3D)source).RaiseEvent(e);
		}
	}

	private static void ShowLockAdorner(DependencyObject targetElement)
	{
		var dependencyObject = GetLockAdornerSite(targetElement);
		if (dependencyObject == null)
		{
			dependencyObject = targetElement;
		}
		ShowLockAdorner(targetElement, dependencyObject);
	}

	private static void ShowLockAdorner(DependencyObject targetElement, DependencyObject adornerSite)
	{
		ShowLockAdorner(targetElement, adornerSite, true);
	}

	private static void ShowLockAdorner(DependencyObject targetElement, DependencyObject adornerSite, bool tryAgain)
	{
		var uIElement = adornerSite as UIElement;
		if (uIElement != null)
		{
			var adornerLayer = AdornerLayer.GetAdornerLayer(uIElement);
			if (adornerLayer == null && tryAgain)
			{
				adornerSite.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action<DependencyObject, DependencyObject, bool>(ShowLockAdorner), targetElement, adornerSite, false);
			}
			else
			{
				var templatedAdorner = uIElement.ReadLocalValue(LockAdornerProperty) as TemplatedAdorner;
				if (templatedAdorner != null)
				{
					if (adornerLayer == null)
					{
						return;
					}
					if (adornerLayer.GetAdorners(uIElement) != null)
					{
						return;
					}
				}
				var lockTemplate = GetLockTemplate(uIElement);
				if (lockTemplate == null)
				{
					lockTemplate = GetLockTemplate(targetElement);
				}
				if (lockTemplate != null)
				{
					templatedAdorner = new TemplatedAdorner(uIElement, GetIsLocked(uIElement), lockTemplate);
					if (adornerLayer != null)
					{
						adornerLayer.Add(templatedAdorner);
					}
					uIElement.SetValue(LockAdornerProperty, templatedAdorner);
				}
			}
		}
	}
}
}