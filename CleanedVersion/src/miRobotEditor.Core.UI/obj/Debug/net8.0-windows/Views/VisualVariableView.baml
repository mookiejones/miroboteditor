   M S B A M L   `   `   `  ����  miRobotEditor.Core.UI2    ,miRobotEditor.UI.Controls.VisualVariableView   5      R NAvalonDock, Version=4.71.1.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J 9http://schemas.microsoft.com/winfx/2006/xaml/presentation       5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5          � 	Resources  5	       �� Source�� 5
   
   $4 ./miRobotEditor.UI;component/Themes/Styles.xaml?�6   %    .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  -   5           
IsReadOnly$
 True��5         DataGridHeaderGlassEffect   ColumnHeaderStyle# �   5   7     DataGridStyle#���  6w     5        tooltip �� Orientation�� 5      %�  &	       _� 5      
�������$��0q�6F   
���   �.��5      �� ��5      �� 5   "   $ 
Horizontal=�6-   .+�5   &   �� ��6,   �� *Icon+�� 5   &   $
��10,0q�6F   $
��Bold"�6T   ��60   �� *Name+6T   5   #   .��5       �� $ Vertical=�6)   .+�5"   "   �� ��6,   �� *Value+�� 5#   "   ��6,   �� *Path+5$      5%      5'      5(         Columns 5+      4  .System.Windows.Controls.DataGridTemplateColumn  5,      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6*      
IsReadOnly$
 True��67      CellTemplate 5-      �� 5.      .R�5/      �� $��16��6<   $��16��6H   	 � ToolTip#	 [ 6S   ��6$   �� *Icon+6S   50      52        54      $
 Auto 6*   $
 True��67   
   Header$

 Name��6I    55      �� 56      .R�57      �� #	 [ 6>   ��6(   �� *Name+6>   58      5:        5;      $
 Auto 6*   $
 True��67   $

 Type��6I    5<      �� 5=      .R�5>      �� #	 [ 6>   ��6(   �� *Type+6>   5?      5A        5B      $
 Auto 6*   $
 True��67   $
 Value��6I    5C      �� 5D      .R�5E      �� #	 [ 6?   ��6(   �� *Value+6?   5F      5H        5I      $ * 6*   $
 True��64   $

 Path��6F    5J      �� 5K      .R�5L      �� #	 [ 6>   ��6(   �� *Path+6>   5M      5O      5R      5T      5U      