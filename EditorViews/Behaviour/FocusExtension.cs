﻿namespace miRobotEditorViews.Behaviour
{
  using System.Windows;

  public static class FocusExtension
  {

    private static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
             "IsFocused", typeof(bool), typeof(FocusExtension),
             new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

    public static bool GetIsFocused(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsFocusedProperty);
    }

    public static void SetIsFocused(DependencyObject obj, bool value)
    {
      obj.SetValue(IsFocusedProperty, value);
    }

    private static void OnIsFocusedPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
    {
      var uie = (UIElement)d;
      if ((bool)e.NewValue)
      {
        uie.Focus(); // Don't care about false values.
      }
    }
  }
}
