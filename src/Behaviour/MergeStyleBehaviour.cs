﻿using miRobotEditor.Properties;

namespace miRobotEditor.Behaviour
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Globalization;

  /// <summary>
  /// This class can be used to adjust styles that are BasedOn another style when
  /// changing a theme at run-time. Normally, styles are not merged. This class
  /// however enables merging of existing partial style definitions in Window/control
  /// XAML with theme specific XAML.
  /// 
  /// Sample
  /// Usage: http://social.msdn.microsoft.com/Forums/da-DK/wpf/thread/63696841-0358-4f7a-abe1-e6062518e3d6
  /// Source: http://stackoverflow.com/questions/5223133/merge-control-style-with-global-style-set-by-another-project-dynamically
  /// </summary>
  public class MergeStyleBehaviour
  {
    #region fields
    /// <summary>
    /// AutoMergeStyle
    /// </summary>
    public static readonly DependencyProperty AutoMergeStyleProperty =
    DependencyProperty.RegisterAttached("AutoMergeStyle", typeof(bool), typeof(MergeStyleBehaviour),
        new FrameworkPropertyMetadata(false,
            OnAutoMergeStyleChanged));

    /// <summary>
    /// BaseOnStyle
    /// </summary>
    public static readonly DependencyProperty BaseOnStyleProperty =
        DependencyProperty.RegisterAttached("BaseOnStyle", typeof(Style), typeof(MergeStyleBehaviour),
            new FrameworkPropertyMetadata(null,
                OnBaseOnStyleChanged));

    /// <summary>
    /// OriginalStyle
    /// </summary>
    public static readonly DependencyProperty OriginalStyleProperty =
                           DependencyProperty.RegisterAttached("OriginalStyle", typeof(Style), typeof(MergeStyleBehaviour),
                           new FrameworkPropertyMetadata((Style)null));
    #endregion fields

    #region public static methods
    /// <summary>
    /// AutoMergeStyle
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool GetAutoMergeStyle(DependencyObject d)
    {
      try
      {
        return (bool)d.GetValue(AutoMergeStyleProperty);
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp.ToString());
      }

      return false;
    }

    /// <summary>
    /// AutoMergeStyle
    /// </summary>
    /// <param name="d"></param>
    /// <param name="value"></param>
    public static void SetAutoMergeStyle(DependencyObject d, bool value)
    {
      try
      {
        d.SetValue(AutoMergeStyleProperty, value);
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp.ToString());
      }
    }

    /// <summary>
    /// BaseOnStyle
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Style GetBaseOnStyle(DependencyObject d)
    {
      try
      {
        return (Style)d.GetValue(BaseOnStyleProperty);
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp.ToString());
      }

      return null;
    }

    /// <summary>
    /// BaseOnStyle
    /// </summary>
    /// <param name="d"></param>
    /// <param name="value"></param>
    public static void SetBaseOnStyle(DependencyObject d, Style value)
    {
      try
      {
        ////Console.WriteLine("Behavior::SetBaseOnStyle");
        d.SetValue(BaseOnStyleProperty, value);
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp.ToString());
      }
    }

    /// <summary>
    /// OriginalStyle
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Style GetOriginalStyle(DependencyObject d)
    {
      try
      {
        return (Style)d.GetValue(OriginalStyleProperty);
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp.ToString());
      }

      return null;
    }

    /// <summary>
    /// OriginalStyle
    /// </summary>
    /// <param name="d"></param>
    /// <param name="value"></param>
    public static void SetOriginalStyle(DependencyObject d, Style value)
    {
      try
      {
        d.SetValue(OriginalStyleProperty, value);
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp.ToString());
      }
    }
    #endregion public static methods

    #region private static methods
    /// <summary>
    /// AutoMergeStyle
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnAutoMergeStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      try
      {
        if (e.OldValue == e.NewValue)
        {
          return;
        }

        var control = d as Control;
        if (control == null)
        {
          throw new NotSupportedException(Resources.MergeStyleBehaviour_OnAutoMergeStyleChanged_AutoMergeStyle_can_only_used_in_Control);
        }

        if ((bool)e.NewValue)
        {
          Type type = d.GetType();
          control.SetResourceReference(BaseOnStyleProperty, type);
        }
        else
        {
          control.ClearValue(BaseOnStyleProperty);
        }
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp.ToString());
      }
    }

    /// <summary>
    /// BaseOnStyle
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnBaseOnStyleChanged(DependencyObject d,
                                             DependencyPropertyChangedEventArgs e)
    {
        if (d == null) return;

      //// if (e.OldValue == null) return;

        try
      {
        if (e.OldValue == e.NewValue)
        {
          return;
        }

        var control = d as Control;
        if (control == null)
        {
          throw new NotSupportedException(Resources.MergeStyleBehaviour_OnBaseOnStyleChanged_BaseOnStyle_can_only_be_used_in_Control);
        }

        var baseOnStyle = e.NewValue as Style;
        var originalStyle = GetOriginalStyle(control);

        if (originalStyle == null)
        {
          originalStyle = control.Style;
          SetOriginalStyle(control, originalStyle);
        }

        var newStyle = originalStyle;

        if (originalStyle.IsSealed)
        {
          ////Console.WriteLine("+++ ORIGINAL STYLE IS SEALED. +++ ");

          newStyle = new Style(originalStyle.TargetType) {Resources = originalStyle.Resources};
          ////newStyle.TargetType = originalStyle.TargetType;

          // 1. Copy resources, setters, triggers
            foreach (var st in originalStyle.Setters)
          {
            newStyle.Setters.Add(st);
          }

          foreach (var tg in originalStyle.Triggers)
          {
            newStyle.Triggers.Add(tg);
          }

          // 2. Set BaseOn Style
          newStyle.BasedOn = baseOnStyle;
        }
        else
          originalStyle.BasedOn = baseOnStyle;

        try
        {
          control.Style = newStyle;
        }
        catch (Exception exp)
        {
          string sInfo = string.Format(CultureInfo.CurrentCulture, Resources.MergeStyleBehaviour_OnBaseOnStyleChanged_newStyle___0_, (newStyle != null ? newStyle.TargetType.FullName : Resources.MergeStyleBehaviour_OnBaseOnStyleChanged__null_));
          sInfo += string.Format(CultureInfo.CurrentCulture, Resources.MergeStyleBehaviour_OnBaseOnStyleChanged_DependencyObject_d___0_, (d));

          Console.WriteLine(exp + Environment.NewLine + Environment.NewLine + sInfo);
        }
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp.ToString());
      }
    }
  }
    #endregion private static methods
}
