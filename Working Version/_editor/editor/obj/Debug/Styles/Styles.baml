   M S B A M L   `   `   `  ����R  NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J FMahApps.Metro, Version=0.15.0.29, Culture=neutral, PublicKeyToken=null  �� MergedDictionaries �� Source   DataGridStyle �� 
TargetType&    System.Windows.Controls.DataGrid �� Property �� Value  HighlightLightBrush#  MahApps.Metro.Controls.Flyout �� 	Resources  BackgroundBrush �� Triggers P� Property P� Value	 :� RoutedEvent
 �� TargetProperty	 W� To g� Duration=  7System.Windows.Controls.Primitives.DataGridColumnHeader  CellEditStyle �� RelativeSource  TextBoxInError �� Self  DataGridHeaderGlassEffect �� 
TargetType � RowDefinitions � ColumnDefinitions �� Stretch  HighlightDarkBrush J� Opacity �� RelativeTransform �� ScaleX �� ScaleY �� AngleX �� AngleY �� Angle T� X T� Y � Color � Offset �� EndPoint �� 
StartPoint  � Cursor!   SeparatorBrush"   SeparatorVisibility# J� RenderTransformOrigin$ �� Color'  !System.Windows.VisualStateManager%   VisualStateGroups%  System.Windows.VisualStateGroup&   Transitions%  System.Windows.VisualTransition'   GeneratedDuration(   States   System.Windows.VisualState)   
Storyboard* �� 
TargetName+ g� 	BeginTime, O� KeyTime- O� Value. R� KeyTime/ R� Value  System.Windows.Visibility�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml      5      6mah,http://metro.mahapps.com/winfx/xaml/controls  5        5      �� 5   
   $ Brushes.xaml?�6   %*  &	        7  '  a    7  '  �    &	     &	     &	 �    �� 5      "   6"   .�5   
   ��  �@  RowBackground �	#FFFFFFFF6*   �� 5	   
     �@  AlternatingRowBackground8   65   �� 5
   
    �@  	RowHeight$ 18Q�6&   �� 5   
    �@  GridLinesVisibility$	 All=�60   �� 5   
    �@  HeadersVisibility$ Column=�6.   �� 5   
     �@  HorizontalGridLinesBrush �	#FFA0A0A065   �� 5   
   % �@  HorizontalScrollBarVisibility$
 Auto=�6:   �� 5   
   # �@  VerticalScrollBarVisibility$
 Auto=�68   �� 5   
    �@  SelectionMode$ Single=�6*   �� 5   
    �@  CanUserReorderColumns . 62   �� 5   
    �@  CanUserResizeColumns . 61   �� 5   
    �@  CanUserSortColumns . 6/   �� 5   
    �@  AutoGenerateColumns .  60   �� 5   
     �@  RowDetailsVisibilityMode$ VisibleWhenSelected=�65   5      �� 5      "  6    5   
   �� 5      �� 5      '��       " ��6   .�5      ��  � ��
 �   �63   5      �� 5      '��       " ��6   .�5      ��  � ��
 �   �63   5       5!      .�5#   
   ��  � ��
 �   �6'   �� 5$   
    � ��8   6'   �� 5%   
    �@J�Opacity$
 0.35Q�6$    5&   
   P� 5'       �@J�IsMouseOver .  . �5(      ��  �@ IsOpen .  6+   5)      :� 5*      $	 UIElement.MouseEnter��6   .[�5+      �� .��5,      �� .��5-      W� $
 Opacity�6*   $	 .95X�6N   $ 0:0:0.5D�6W   5.      5/      50      :� 51      $	 UIElement.MouseLeave��6   .[�52      �� .��53      �� .��54      W� $
 Opacity�6*   $	 .25X�6N   $ 0:0:0.5D�6W   55      56      57      59      �� 5;      "  6   .�5=   
   ��  � ��$ 5,0,5,0q�6$   �� 5>   
    � ��
 �   �6'   5@      �� 5A      " ��6"   .�5B   
   ��  � ��$ 0q�6,   �� 5C   
    � ��$ 0q�6$    5D   
   P� 5E       �@=�HasError . . �5F      ��  �@�ToolTip 5G      �� *%#(Validation.Errors)[0].ErrorContent+ �� *Self+5H      5J      �� 5N      " ��6#    5O   
   P� 5P       �@=�HasError . . �5Q      ��  �@�ToolTip 5R      �� *%#(Validation.Errors)[0].ErrorContent+# Z 5S      5U      �� 5V      "  6.   .�5W   
   ��  � ��$ 11(�6%   �� 5X   
    � �� �	#FFEEEEEE6'   �� 5Y   
    � �� 5Z      �� 5[      "  6"   .R�5\      � $
��Root��6    5]      �� 5^      $��* �6,   �� 5_      $��* �6,   �� 5`      $
��Auto �6,    5b      �� 5c      $
��Auto �6/   �� 5d      $��* �6/   �� 5e      $
��Auto �6/   .+�5g      �� $��BackgroundRectangle��6$   $
 Fill=�6?   #���  5h   $   $��2��6P   $��2��6d   �� 5i      ���	#00FFFFFF$��1,1,1,1q�69   $��3��6S   $��3��6g   ._�5j      �  5k   "   �� 5l   &   $
��0.5* �64   �� 5m   &   $
��0.5* �64   .+�5o   "   �� $
��dark��6)   $��Stretch=�65   $��0,0,0,0q�6S   $
��Auto��6d   $��3��5p   )   $��3��6=   ���	#66000000$ 0Q�6e   �� 5q   "   $
��glow��6)   $ 0Q�65   $��Stretch=�6A   $
��Auto��6_   $��2��5r   )   $��3��6:   ��5s   &   
� 5t   *    5u   .   V� 5v   2   .~�5w   6   �� $	 1.7Q�6E   $	 2.2Q�6R   �� 5x   6   $ 0Q�6D   $ 0Q�6O   �� 5y   6   $ 0Q�6F   T� 5z   6   $
 -0.3Q�6I   $
 -0.1Q�6R   5{   3   .��5}   .   � $ 	#B2FFFFFF��6;   $ 0Q�6M   � 5~   .   $ 	#00FFFFFF��6;   $ 1Q�6M   5   +   5�   #   �� 5�   "   $��shine��6)   $��Stretch=�66   $��0,0,0,0q�6T   $
��Auto��6e   $��3��5�   )   ��5�   &   �� 5�   *   $ 0.5,0.9&�6>   $ 0.5,0.1&�6Q   .��5�   .   � $ 	#99FFFFFF��6;   $ 0Q�6M   � 5�   .   $ 	#33FFFFFF��6;   $ 1Q�6M   5�   +   5�   #   5�      5�      �� 5�      $��
HeaderText��6+   $��0��6=   $��2��6J   #��z��5�   +   #  z  5�   +   #��z��5�   +   #��z��5�   +   $��10,0,0,0q�5�   +   �� 5�      $��VerticalSeparator��6$   $��2��6=   $��2��6N   $��1��6^   $��Stretch=�5�   $   #��z! 5�   $   #y�z" 5�   $   F� 5�      $��SortIcon��6   $��2��6/   $# .5,.5&�6@   $
��Left=�6^   $��Center=�5�      $ 0Q�6:   $��1��6F   $ Uniform=�6V   $��8��6h   "����@9����- ���- ��- �        ��5�      �� 5�   "   $$ 	#FFFFFFFF��62   z�5�      V� 5�   "   .~�5�   &   �� SortIconTransform��65   5�   #   5�      % 5�        5�      
SortStates��& 5�   "     5�   &   $' 	00:00:0.1D�67   .( 5�   "     Unsorted��6.     5�   "   SortAscending��.) 5�   &   �� .��5�   *   W� $* SortIcon��6:   $
 Opacity�5�   :   $ 0D�6^   $	 1.0X�6k   5�   '   5�   #     5�   "   SortDescending��.) 5�   &   �� .��5�   *   W� $* SortIcon��6:   $
 Opacity�5�   :   $ 0D�6^   $	 1.0X�6k   W� 5�   *   $* SortIconTransform��6:   $
 ScaleY�5�   :   $ 0D�6]   $ -1X�6j   5�   '   5�   #   5�        5�      CommonStates��.( 5�   "     
Normal��6.     5�   "   	MouseOver��.) 5�   &   �� .��5�   *   U� $+ 00:00:00X�6H   $
* glow��5�   H   $
 (UIElement.Opacity)�5�   H   .\�5�   .   �� $, 00:00:00.4000000��6C   $- 1Q�6^   5�   +   5�   '   5�   #     5�   "   Pressed��.) 5�   &   �� .��5�   *   U� $* shine��6H   $
 Opacity�5�   H   .\�5�   .   �� $, 0��6C   $- 0Q�6O   5�   +   U� 5�   *   $+ 00:00:00X�6H   $
* dark��5�   H   $
 (UIElement.Opacity)�5�   H   .\�5�   .   �� $, 00:00:00.0000000��6C   $- 1Q�6^   5�   +   T� 5�   *   $+ 00:00:00X�6H   $ 00:00:00.0010000D�6]   $
* glow��5�   H   $
 (UIElement.Visibility)�5�   H   ..�5�   .   i� $. 00:00:00��6E   / 5�   2    5�   6   	Collapsed=�6L   5�   /   5�   +   5�   '   5�   #   5�      5�      5�      5�      5�      5�      