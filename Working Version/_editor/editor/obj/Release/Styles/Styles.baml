   M S B A M L   `   `   `  ���� L  HmiRobotEditor, Version=0.0.0.21359, Culture=neutral, PublicKeyToken=nullA%clr-namespace:miRobotEditor.ViewModelmiRobotEditor.ViewModel  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J FMahApps.Metro, Version=0.15.0.29, Culture=neutral, PublicKeyToken=null  �� MergedDictionaries �� Source   DataGridStyle �� 
TargetType&    System.Windows.Controls.DataGrid �� Property �� Value  HighlightLightBrush#  MahApps.Metro.Controls.Flyout �� 	Resources  BackgroundBrush �� Triggers P� Property P� Value	 :� RoutedEvent
 �� TargetProperty	 W� To g� Duration=  7System.Windows.Controls.Primitives.DataGridColumnHeader  CellEditStyle �� RelativeSource  TextBoxInError �� Self  DataGridHeaderGlassEffect �� 
TargetType � RowDefinitions � ColumnDefinitions �� Stretch  HighlightDarkBrush J� Opacity �� RelativeTransform �� ScaleX �� ScaleY �� AngleX �� AngleY �� Angle T� X T� Y � Color � Offset �� EndPoint �� 
StartPoint  � Cursor!   SeparatorBrush"   SeparatorVisibility# J� RenderTransformOrigin$ �� Color'  !System.Windows.VisualStateManager%   VisualStateGroups%  System.Windows.VisualStateGroup&   Transitions%  System.Windows.VisualTransition'   GeneratedDuration(   States   System.Windows.VisualState)   
Storyboard* �� 
TargetName+ g� 	BeginTime, O� KeyTime- O� Value. R� KeyTime/ R� Value  System.Windows.Visibility�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     6mah,http://metro.mahapps.com/winfx/xaml/controls  3	viewModel%clr-namespace:miRobotEditor.ViewModel    �� $ Brushes.xaml?�%�  &	        7  '  �    7  '  B    &	 �    &	 5    &	 �    �� "   .���  �@  RowBackground �	#FFFFFFFF��   �@  AlternatingRowBackground8   ��  �@  	RowHeight$ 18Q���  �@  GridLinesVisibility$	 All=���  �@  HeadersVisibility$ Column=���   �@  HorizontalGridLinesBrush �	#FFA0A0A0�� % �@  HorizontalScrollBarVisibility$
 Auto=��� # �@  VerticalScrollBarVisibility$
 Auto=���  �@  SelectionMode$ Single=���  �@  CanUserReorderColumns . ��  �@  CanUserResizeColumns . ��  �@  CanUserSortColumns . ��  �@  AutoGenerateColumns .  ��   �@  RowDetailsVisibilityMode$ VisibleWhenSelected=��� "   �� �� '��       " ��.���  � ��
 �   ��� '��       " ��.���  � ��
 �   �.���  � ��
 �   ���  � ��8   ��  �@J�Opacity$
 0.35Q� P�  �@J�IsMouseOver .  . ���  �@ IsOpen .  :� $	 UIElement.MouseEnter��.[��� .���� .��W� $
 Opacity�$	 .95X�$ 0:0:0.5D�:� $	 UIElement.MouseLeave��.[��� .���� .��W� $
 Opacity�$	 .25X�$ 0:0:0.5D��� "  .���  � ��$ 5,0,5,0q���  � ��
 �   ��� " ��.���  � ��$ 0q���  � ��$ 0q� P�  �@=�HasError . . ���  �@�ToolTip �� *%#(Validation.Errors)[0].ErrorContent+ �� *Self+�� " �� P�  �@=�HasError . . ���  �@�ToolTip �� *%#(Validation.Errors)[0].ErrorContent+# Z �� "  .���  � ��$ 11(���  � �� �	#FFEEEEEE��  � �� �� "  .R�� $
��Root�� �� $��* ��� $��* ��� $
��Auto � �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��BackgroundRectangle��$
 Fill=�#���  $��2��$��2���� ���	#00FFFFFF$��1,1,1,1q�$��3��$��3��._��  �� $
��0.5* ��� $
��0.5* �.+��� $
��dark��$��Stretch=�$��0,0,0,0q�$
��Auto��$��3��$��3�����	#66000000$ 0Q��� $
��glow��$ 0Q�$��Stretch=�$
��Auto��$��2��$��3����
�  V� .~��� $	 1.7Q�$	 2.2Q��� $ 0Q�$ 0Q��� $ 0Q�T� $
 -0.3Q�$
 -0.1Q�.��� $ 	#B2FFFFFF��$ 0Q�� $ 	#00FFFFFF��$ 1Q��� $��shine��$��Stretch=�$��0,0,0,0q�$
��Auto��$��3������ $ 0.5,0.9&�$ 0.5,0.1&�.��� $ 	#99FFFFFF��$ 0Q�� $ 	#33FFFFFF��$ 1Q��� $��
HeaderText��$��0��$��2��#��z��#  z  #��z��#��z��$��10,0,0,0q��� $��VerticalSeparator��$��2��$��2��$��1��$��Stretch=�#��z! #y�z" F� $��SortIcon��$��2��$# .5,.5&�$
��Left=�$��Center=�$ 0Q�$��1��$ Uniform=�$��8��"����@9����- ���- ��- �        ���� $$ 	#FFFFFFFF��z�V� .~��� SortIconTransform��%   
SortStates��&   $' 	00:00:0.1D�.(   Unsorted��  SortAscending��.) �� .��W� $* SortIcon��$
 Opacity�$ 0D�$	 1.0X�  SortDescending��.) �� .��W� $* SortIcon��$
 Opacity�$ 0D�$	 1.0X�W� $* SortIconTransform��$
 ScaleY�$ 0D�$ -1X�  CommonStates��.(   
Normal��  	MouseOver��.) �� .��U� $+ 00:00:00X�$
* glow��$
 (UIElement.Opacity)�.\��� $, 00:00:00.4000000��$- 1Q�  Pressed��.) �� .��U� $* shine��$
 Opacity�.\��� $, 0��$- 0Q�U� $+ 00:00:00X�$
* dark��$
 (UIElement.Opacity)�.\��� $, 00:00:00.0000000��$- 1Q�T� $+ 00:00:00X�$ 00:00:00.0010000D�$
* glow��$
 (UIElement.Visibility)�..�i� $. 00:00:00��/  	Collapsed=�