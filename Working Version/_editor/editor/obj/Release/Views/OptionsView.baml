   M S B A M L   `   `   `  ���� L  HmiRobotEditor, Version=0.0.0.21359, Culture=neutral, PublicKeyToken=null=#clr-namespace:miRobotEditor.ClassesmiRobotEditor.Classes  %    miRobotEditor.Views.OptionsView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     X TXceed.Wpf.Toolkit, Version=2.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  J FMahApps.Metro, Version=0.15.0.29, Culture=neutral, PublicKeyToken=null;
converters*http://metro.mahapps.com/winfx/xaml/shared  -gui#clr-namespace:miRobotEditor.Classes    � 	Resources  9  3MahApps.Metro.Converters.ThicknessToDoubleConverter   ThicknessToDoubleConverter2  ,System.Windows.Controls.AlternationConverter  AlternationConverterG  AXceed.Wpf.Toolkit.Core.Converters.SolidColorBrushToColorConverter # SolidColorBrushToColorConverterG  AXceed.Wpf.Toolkit.Core.Converters.ColorToSolidColorBrushConverter # ColorToSolidColorBrushConverter��%<   &	        &	      &	 
     &	              .��� .+��� .@��� )   #miRobotEditor.Classes.GlobalOptions   Instance � DataContext# Z ���� *Title+.��R� ���� .@�Q� $��Flyout��.@���  �� Orientation$ 
Horizontal=�.+��� $��Opacity��$  Xceed.Wpf.Toolkit.DoubleUpDown  �  �Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	Increment$	 0.1X��  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Maximum$ 1X�   Minimum$ 0X�   Value �� *FlyoutOpacity+�� $��Text Editor Options��)	   #miRobotEditor.Classes.EditorOptions 	  Instance# Z   	 �� 
TargetType
 �� Property �� Value#
  Xceed.Wpf.Toolkit.ColorPicker��%�   '��       '
  t     �� "	 ��.��� 
 � ��$
 10,0q��� 
 �@J�IsHitTestVisible .  �� 
 �@�OverridesDefaultStyle . �� "	 
 .��� 
 � ��$
 10,0q��� 
 �@
 ShowAvailableColors . �� 
 �@
 DisplayColorAndName . .���� $ Vertical=�$
��10,0q�.+��� $$��Allow Scrolling Below Document�� d� 	IsChecked �� *AllowScrollingBelowDocument+�� $��Convert Tabs to spaces.�� �� *ConvertTabsToSpaces+�� $��Cut/Copy Whole Line�� �� *CutCopyWholeLine+�� $��Enable Animations�� �� *EnableAnimations+�� $��Enable Email Hyperlinks�� �� *EnableEmailHyperlinks+�� $��Enable Folding�� �� *EnableFolding+�� ���� $��Enable Hyperlinks�� �� *EnableHyperlinks+.���� $+��%Require Control Button For Hyperlinks��{��� *EnableHyperlinks+ �� *)'RequireControlModifierForHyperlinkClick+�� $��	Timestamp��.���  � ColumnDefinitions �� ��  � RowDefinitions �� �� .+��� $��Format���� $��0��$��1������ *TimestampFormat+ �� Mode$ TwoWay=� �� NotifyOnSourceUpdated .  �� UpdateSourceTrigger$ PropertyChanged=��� $��Sample��$��1��$��0���� $��1��$��1������ *TimestampSample+�� $"��Enable Rectangular Selection�� �� *EnableRectangularSelection+�� $��Enable Text Drag/Drop�� �� *EnableTextDragDrop+�� $��Enable Virtual Space�� �� *EnableVirtualSpace+�� $��Highlight current line�� �� *HighlightCurrentLine+�� $$��Allow Zooming With Mouse Wheel�� �� *MouseWheelZoom+�� $%��Show Box For Control Characters�� �� *ShowBoxForControlCharacters+�� $��Highlight current line��.���� $ Vertical=�.+��� $ 
Horizontal=�.+��� $��Selected Line Color��
  
 � Tag$ 
Font Color�� 
  SelectedColor �� *HighlightedLineColor+�� ���� *WordWrapIndentation+�� $��Show End of Line�� �� *ShowEndOfLine+�� $��Show Line Numbers�� �� *ShowLineNumbers+�� $��Show Spaces�� �� *
ShowSpaces+�� $��
Wrap words�� �� *	WrapWords+�� $��Editor Colors��.���� $ Vertical=�.+��� $ 
Horizontal=�.+��� $��Line Numbers��
  $ 
Font Color�� �� *LineNumbersFontColor+�� $ 
Horizontal=�.+��� $��
Font Color��
  $ 
Font Color�� �� *	FontColor+�� $ 
Horizontal=�.+��� $��Background Color��
  $ Background Color�� �� *BackgroundColor+�� $��Fold ToolTip��.���� $ Vertical=�.+��� $ 
Horizontal=�.+��� $��Background Color��
  $ Background Color�� �� *FoldToolTipBackgroundColor+�� $ 
Horizontal=�.+��� $��Border Color��
  $ Background Color�� �� *" FoldToolTipBackgroundBorderColor+�� $ 
Horizontal=�.+��� $��BorderThickness Color��   �� *FoldToolTipBorderThickness+�� $��Selected Text��.���� $ Vertical=�.+��� $ 
Horizontal=�$��Center=�.+��� $��
Font Color��
   �� *SelectedFontColor+�� $ 
Horizontal=�$��Center=�.+��� $��Background Color��
   �� *SelectedTextBackground+�� $ 
Horizontal=�$��Center=�.+��� $��Border Color��
   �� *SelectedBorderColor+�� $��Border Thickness��     DefaultValue$ 1X� �� *SelectedBorderThickness+