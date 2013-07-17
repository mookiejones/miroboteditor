using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace TestApplication
{
    public class WatermarkTextBox:TextBox,INotifyPropertyChanged
    {
        private Brush _inactiveBorderBrush = SystemColors.InactiveBorderBrush;
        
        public static readonly DependencyProperty InactiveBorderBrushProperty=DependencyProperty.Register("InactiveBorderBrush",typeof(Brush),typeof(WatermarkTextBox),new PropertyMetadata(SystemColors.InactiveBorderBrush));
        public Brush InactiveBorderBrush { get { return (Brush)GetValue(InactiveBorderBrushProperty); } set { SetValue(InactiveBorderBrushProperty, value); } }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header",typeof(string),typeof(WatermarkTextBox));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(WatermarkTextBox));

        public string Value { get { return (string)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public string Header { get { return (string)GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }

        public string _displayedText = string.Empty;
        public string DisplayedText { get { return _displayedText; } set { _displayedText = value; RaisePropertyChanged("DisplayedText"); } }


        public WatermarkTextBox()
        {
            this.Text = String.Format("{0} := {1}", Header, Value);
            this.GotFocus += delegate { Text = Value; };
            LostFocus += delegate { Text = String.Format("{0} := {1}",Header,Value); Background = SystemColors.InactiveBorderBrush; };
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged!=null)
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }
    }
}
