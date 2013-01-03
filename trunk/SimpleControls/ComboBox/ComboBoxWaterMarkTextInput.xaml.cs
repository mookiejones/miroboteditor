﻿namespace SimpleControls.ComboBox
{
  using System.Collections;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for ComboBoxWithLabel.xaml
  /// </summary>
  public partial class ComboBoxWaterMarkTextInput : UserControl
  {
    #region fields
    #region ComboBox
    private static readonly DependencyProperty ItemsSourceProperty =
      ComboBox.ItemsSourceProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

    private static readonly DependencyProperty LabelContentProperty =
      DependencyProperty.Register("LabelContent", typeof(string), typeof(ComboBoxWaterMarkTextInput));

    private static readonly DependencyProperty DisplayMemberPathProperty =
      ComboBox.DisplayMemberPathProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

    private static readonly DependencyProperty SelectedValuePathProperty =
      ComboBox.SelectedValuePathProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

    private static readonly DependencyProperty SelectedItemProperty =
      ComboBox.SelectedItemProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

    private static readonly DependencyProperty SelectedValueProperty =
      ComboBox.SelectedValueProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

    private static readonly DependencyProperty SelectedIndexProperty =
      ComboBox.SelectedIndexProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));
    #endregion ComboBox

    #region TextBox
    private static readonly DependencyProperty LabelTextBoxProperty =
      DependencyProperty.Register("LabelTextBox", typeof(string), typeof(ComboBoxWaterMarkTextInput));

    private static readonly DependencyProperty TextProperty =
      TextBox.TextProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

    private static readonly DependencyProperty WatermarkProperty =
      DependencyProperty.Register("Watermark", typeof(string), typeof(ComboBoxWaterMarkTextInput));
    #endregion TextBox
    #endregion fields

    #region constructor
    public ComboBoxWaterMarkTextInput()
    {
      this.InitializeComponent();
    }
    #endregion constructor

    #region properties
    #region ComboBox
    /// <summary>
    /// Declare ItemsSource and Register as an Owner of ComboBox.ItemsSource
    /// the ComboBoxWaterMarkTextInput.xaml will bind the ComboBox.ItemsSource to this property
    /// </summary>
    public IEnumerable ItemsSource
    {
      get { return (IEnumerable)GetValue(ComboBoxWaterMarkTextInput.ItemsSourceProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.ItemsSourceProperty, value); }
    }

    /// <summary>
    /// Declare a ComboBox label dependency property
    /// </summary>
    public string LabelContent
    {
      // These proeprties can be bound to. The XAML for this control binds the Label's content to this.
      get { return (string)GetValue(ComboBoxWaterMarkTextInput.LabelContentProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.LabelContentProperty, value); }
    }

    /// <summary>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
    /// </summary>
    public string DisplayMemberPath
    {
      get { return (string)GetValue(ComboBoxWaterMarkTextInput.DisplayMemberPathProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.DisplayMemberPathProperty, value); }
    }

    /// <summary>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
    /// </summary>
    public string SelectedValuePath
    {
      get { return (string)GetValue(ComboBoxWaterMarkTextInput.SelectedValuePathProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.SelectedValuePathProperty, value); }
    }

    /// <summary>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
    /// </summary>
    public object SelectedItem
    {
      get { return (object)GetValue(ComboBoxWaterMarkTextInput.SelectedItemProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.SelectedItemProperty, value); }
    }

    /// <summary>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
    /// </summary>
    public object SelectedValue
    {
      get { return (object)GetValue(ComboBoxWaterMarkTextInput.SelectedValueProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.SelectedValueProperty, value); }
    }

    /// <summary>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
    /// </summary>
    public int SelectedIndex
    {
      get { return (int)GetValue(ComboBoxWaterMarkTextInput.SelectedIndexProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.SelectedIndexProperty, value); }
    }
    #endregion ComboBox

    #region TextBox
    /// <summary>
    /// Declare a TextBox label dependency property
    /// </summary>
    public string LabelTextBox
    {
      // These proeprties can be bound to. The XAML for this control binds the Label's content to this.
      get { return (string)GetValue(ComboBoxWaterMarkTextInput.LabelTextBoxProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.LabelTextBoxProperty, value); }
    }

    /// <summary>
    /// Declare a TextBox Text dependency property
    /// </summary>
    public string Text
    {
      // These proeprties can be bound to. The XAML for this control binds the Label's content to this.
      get { return (string)GetValue(ComboBoxWaterMarkTextInput.TextProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.TextProperty, value); }
    }

    /// <summary>
    /// Declare a TextBox Watermark label dependency property
    /// </summary>
    public string Watermark
    {
      // These proeprties can be bound to. The XAML for this control binds the Watermark's content to this.
      get { return (string)GetValue(ComboBoxWaterMarkTextInput.WatermarkProperty); }
      set { SetValue(ComboBoxWaterMarkTextInput.WatermarkProperty, value); }
    }
    #endregion TextBox
    #endregion properties
  }
}
