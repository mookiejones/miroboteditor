   M S B A M L   `   `   `  ���� F  BKRC4Options, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:KRC4OptionsKRC4Options      KRC4Options.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     1krc4Optionsclr-namespace:KRC4Options       � Title$  
MainWindow��$	��350��$	��525�� � 	Resources  �� MergedDictionaries �� Source"   KRC4Options.EnvPathToBoolean   EnvPathToBoolean��  �� $ KUKAResources.xaml?�%   &	           � DataContext    KRC4Options.MainViewModel  .��� $��10q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto � � ColumnDefinitions �� $
��Auto ��� $��* �.+��� $��Customer��$��0���� $��1��$��0������ * Config.CustomerVariant.VarName+�� $��KSS Version��$��1��$��0���� $��1��$��1������ *Config.Version.Info.Version+�� $��0��$��2��$��2��$��1q�
���   �._��� $��Authentication��.��� .+��� $��10q�$��5q����� *PNAuthentication.authenticationManagement.AuthenticationConfiguration.MethodList+����  �� Mode$ OneWay=�  
MethodList �� Source# [ �� $��0��$��3��$��2��$��
IsOfficePC��	 d� 	IsChecked	 �� *Config.Environment.EnvPath+
 �� 	Converter#
 [  