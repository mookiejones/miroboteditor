   M S B A M L   `   `   `  ����F  BKRC4Options, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:KRC4OptionsKRC4Options  5   	       KRC4Options.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   1krc4Optionsclr-namespace:KRC4Options     5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)    � 	Resources 5       �� MergedDictionaries �� Source"   KRC4Options.EnvPathToBoolean   EnvPathToBoolean�� 5   
    5	      �� 5
      $ KUKAResources.xaml?�6%   %   &	          5      6+   5       � DataContext 5         KRC4Options.MainViewModel  5   
   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��Customer��6   $��0��6$   �� 5   
   $��1��6   $��0��6"   ��6/   �� * Config.CustomerVariant.VarName+�� 5    
   $��KSS Version��6   $��1��6'   $��0��64   �� 5!   
   $��1��6   $��1��6   ��6/   �� *Config.Version.Info.Version+�� 5"   
   $��0��6   $��2��6!   $��2��6.   $��1q�6B   
���   �._�5#      �� $��Authentication��6   .��5$      � .+�5%      �� $��10q�6   $��5q�6+   ��5&      �� *PNAuthentication.authenticationManagement.AuthenticationConfiguration.MethodList+��5'      ��  �� Mode$ OneWay=�  
MethodList �� Source# [ 5(      5)      5*      �� 5+   
   $��3��6   $��2��6    $��
IsOfficePC��64   	 d� 	IsChecked	 5,      �� *Config.Environment.EnvPath+
 �� 	Converter#
 [  5-      5.      