   M S B A M L   `   `   `  ����L  HmiRobotEditor, Version=0.0.0.27371, Culture=neutral, PublicKeyToken=nullA%clr-namespace:miRobotEditor.ViewModelmiRobotEditor.ViewModel  5   	   '    !miRobotEditor.Windows.AboutWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ;	viewModel%clr-namespace:miRobotEditor.ViewModel     5   	     � Title$  AboutWindow��5   	   $	��531��6   $	��430��6*   $��10q�66    � 
ResizeMode$ NoResize=�6C   ��5      �� 5   
    � MappingMode$ Absolute=�6    �� 
StartPoint$	 0,0&�65    �� EndPoint$ 0,180&�6F   .��5      �  � Color$ 	#FFDEDEDE��6   � 5	      $ White��6    � Offset$ 1Q�6)   5
      .��5      ��  �� Orientation$ Vertical=�6    � DataContext 5   
   ,   &miRobotEditor.ViewModel.AboutViewModel  5      .+�5   
   �� $	��225��6   .��5      �� $��/Resources\Editor.png��6   $��48��63   $��48��6?   5      �� 5   
   ��6   �� *	Copyright+�� 5   
   .@�5      �� ��6   �� *Name+	 �� StringFormat$	 	About {0}��.��5      � 
 � RowDefinitions
 5      �� 5      �� 5       � ColumnDefinitions 5      �� 5      �� 5      �� 5      �� 5      .+�5!      �� $��Version��6    $��Stretch=�6/   �� 5"      $��1��6    ���	#FF808080��6G   �� *	Version+�� 5#      $��1��6    $��4��6-   ��6A   �� *Info+5$      5%      5&      �� 5'   
   -   5        $��Close��5'      $��Bottom=�6!   6<   5(      5)      