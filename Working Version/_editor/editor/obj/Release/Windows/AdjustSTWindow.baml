   M S B A M L   `   `   `  ���� L  HmiRobotEditor, Version=0.0.0.21359, Culture=neutral, PublicKeyToken=nullA%clr-namespace:miRobotEditor.ViewModelmiRobotEditor.ViewModel  *    $miRobotEditor.Windows.AdjustSTWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     ;	viewModel%clr-namespace:miRobotEditor.ViewModel       � Title$  frmAdjustST��$	��793������  �� Color# � �� � 	Resources &   miRobotEditor.ViewModel.AdjustST   AdjustSt��%   &	          .���  � ColumnDefinitions �� $
��337* ��� $
��415* �.+��� $��	Positions��$��0��.���  � RowDefinitions �� $��25 ��� $��50 ��� $��2* ��� $��25 ��� $��5* � �� $	��113 ��� $��97 ��� $	��112 ��� .+��� $��List positions from��$��0��$��20��$��0,5,0,0q�$��Stretch=� �� TextAlignment$ Center=�$	��Top=��� -   $��ComboBox��$��1��$��0��$��25��$	��Top=�$��Stretch=����	#00FFFFFF$��3��.@��� $��Actual Selection���� $��
Actual SRC���� $��
Actual DAT���� $��	All SRC's���� $��	All DAT's���� $��Filter��$��1��$��0��$��3��$��Stretch=�$��Stretch=�.���  �� $	��25* ��� $	��90* ��� $	��27* ��� $��40 ��� $��35 ��� $��40 ��� $��5* �.+��� $
��Name��$��Center=�$	��Top=�$��0��$��16���� $��Stretch=�$	��Top=�$��1��$��1��$��22���� $��Tool[��$��Center=�$	��Top=�$��2��$��1��$��16���� $��3��$	��Top=�$��22������  �� Mode$ OneWay=� �� Source# [   �� SelectedValuePath �� $ OneWay=��� $��]Base[��$��Center=�$	��Top=�$��4��$��1��$��16���� $��5��$	��Top=�$��22������ $ OneWay=�# [   �� $ OneWay=��� $��]��$	��Top=�$��6��$��1��$��16���� $
��Left=�$��2��$��3��$	��320��$��Stretch=�	 �� View	 �� .P��� 
 �� Width$
 60��$
��Name���� $
 80��$��Declaration���� $
 60��$
��Tool���� $
 60��$
��Base���� $
 60��$��Value���� $��
Select All��$��3��$	��100��$��0��$��0,0,13,0q��� $��Invert Selection��$��3��$��Stretch=�$��1��$��1���� $��3��$��2��$��Selected(0/0)��$��1��$ Center=�$	��0,5q��� $��Turn Adjustment��$��1��