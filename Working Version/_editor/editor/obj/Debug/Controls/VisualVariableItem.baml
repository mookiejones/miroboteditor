   M S B A M L   `   `   `  ����  miRobotEditor/    )miRobotEditor.Controls.VisualVariableItem   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5        &   System.Windows.Controls.DataGrid    
IsReadOnly  .    DataGridHeaderGlassEffect   ColumnHeaderStyle# �   5        DataGridStyle#���  5       � 	Resources 5       �� MergedDictionaries �� Source  tooltip �� Orientation�� 5   
    5      �� 5      $1 +/miRobotEditor;component/Styles/Styles.xaml?�6%   %�  &	       _� 5      ���	#FFF8F8FF$��0q�6=   ���	#FF000000.��5      �� ��5      �� 5      $ 
Horizontal=�6%   .+�5      �� ��6$   �� *Icon+�� 5      $
��10,0q�6(   $
��Bold"�66   ��6R   �� *Name+5      .��5      �� $ Vertical=�6!   .+�5      �� ��6$   �� *Value+�� 5      ��6$   �� *Path+5      5      5      5         Columns 5       4  .System.Windows.Controls.DataGridTemplateColumn  5!   
   ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6!      
IsReadOnly . 	   CellTemplate	 5"      �� 5#      .R�5$      �� $��16��6   $��16��6(   
 � ToolTip#
 [ 63   ��6V   �� *Icon+5%      5'        5(   
   $
 Auto 6!    .    Header$
 Name��6@   	 5)      �� 5*      .R�5+      �� #
 [ 6    ��6C   �� *Name+5,      5.        5/   
   $
 Auto 6!    . $
 Type��6@   	 50      �� 51      .R�52      �� #
 [ 6    ��6C   �� *Type+53      55        56   
   $
 Auto 6!    . $ Value��6@   	 57      �� 58      .R�59      �� #
 [ 6    ��6C   �� *Value+5:      5<        5=   
   $ * 6!    . $
 Path��6=   	 5>      �� 5?      .R�5@      �� #
 [ 6    ��6C   �� *Path+5A      5C      5E      