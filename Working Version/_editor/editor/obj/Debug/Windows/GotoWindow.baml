   M S B A M L   `   `   `  ����  miRobotEditor&     miRobotEditor.Windows.GotoWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      J FMahApps.Metro, Version=0.15.0.29, Culture=neutral, PublicKeyToken=null;controls,http://metro.mahapps.com/winfx/xaml/controls  5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      ;
behaviours*http://metro.mahapps.com/winfx/xaml/shared  5        � Title$  
Go To Line��5       � 
ResizeMode$ NoResize=�6*    � ShowInTaskbar .  $	��140��6V   $��1q�6c   $	��200��5      $��Stretch=�6#   $��Stretch=�6A    � ShowActivated .  ��5	      �� 5
   
    � MappingMode$ Absolute=�6    �� 
StartPoint$	 0,0&�65    �� EndPoint$ 0,180&�6F   .��5      �  � Color$ 	#FFDEDEDE��6   � 5      $ White��6    � Offset$ 1Q�6)   5      .  (System.Windows.Interactivity.Interaction	   	Behaviors	 5      7  1MahApps.Metro.Behaviours.BorderlessWindowBehavior  5   
   .��5      �� 
 �� Orientation$
 Vertical=�6    � 	Resources 5   
    �� 
TargetType �� Property �� Value��%�   '��       �� 5      " ��6,   .�5      ��  � ��$ Stretch=�66   �� 5       � ��$ 75��6*   �� 5       � ��$	 5,0q�6+   �� 5       � ��$ Right=�68   5      5      .+�5   
   �� $��Stretch=�6   $��5q�62   $
��Wrap=�6=   $��20��6Q   ��5      �� *%#Main.ActiveEditor.TextBox.LineCount+ �� StringFormat$&  Enter a Number between 0 and {0}�� �� Source �� *	Locator+�� 5   
   -   5        $��EnteredText��5      $��1��6%    �� TextAlignment$ Center=�62   $��Stretch=�6I   $��5q�6g    �� TextWrapping$
 Wrap=�5      $��Stretch=�6&   $��25��6B   ��6N   �� *EnteredText+�� 5   
   $
 
Horizontal=�6   $��Right=�6.   $��5q�6J   .+�5       �� -   5        $	��_OK��5       ��6#   �� *	OkCommand+6A   �� 5!      -   5        $��_Cancel��5!      6'   5"      5#      5$      