   M S B A M L   `   `   `  ���� L  HmiRobotEditor, Version=0.0.0.21359, Culture=neutral, PublicKeyToken=null?$clr-namespace:miRobotEditor.ControlsmiRobotEditor.Controls  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   ExtendedTextBlock �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value �� ScaleY	 �� ScaleX # VerticalGridSplitterHandleStyle
 �� 
TargetType'   !System.Windows.VisualStateManager    VisualStateGroups%  System.Windows.VisualStateGroup   Transitions%  System.Windows.VisualTransition   GeneratedDuration	   To   States   System.Windows.VisualState   
Storyboard �� 
TargetName �� TargetProperty g� Duration	 W� To �� CornerRadius J� Opacity J� RenderTransformOrigin �� Angle % !HorizontalGridSplitterHandleStyle  
LabelStyle	 �� To � 	Resources
 � Tag  SwitchArrowStyle  GridSplitterPreviewStyle1   +miRobotEditor.Controls.ExtendedGridSplitter J� IsHitTestVisible � ColumnDefinitions   HorizontalHandleStyle � RowDefinitions    VerticalHandleStyle�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     1controls$clr-namespace:miRobotEditor.Controls    �� $2 ,/miRobotEditor;component/Styles/Brushes.xaml?�%�  &	        &	 Z    &	 �    &	 �    7   &	 �    &	 �    '  {    7  7  7  7  7  7  �� " ��.���  � ��$ Center=���  � ��$ Center=���  �@J�IsHitTestVisible .  ��  �@J�Opacity$ 1Q���  � �� �	#FFF5F5F5��  �@J�RenderTransformOrigin$ 0.5,0.5&� P�  �@J�IsMouseOver . . ���  � �� �	#FFFFFFFFP�  � ��$
 Data��. ���  � z� �� $ -1Q�$	 -1Q��� " d�.���  �@�Cursor$
 Hand����  �@d�	IsChecked$ FalseY���  � �� �� "
 d�.R�� $
��Root��   CommonStates��   $ 0:0:00.2D�$ 	MouseOver��  $ 0:0:00.2D�$ Normal��.   
Normal��. �� .��W� $ BackgroundBorder��$ Opacity�$ 
00:00:00.3D�$	 0.4X�  	MouseOver��. �� .��W� $ BackgroundBorder��$ Opacity�$ 
00:00:00.3D�$ 1X�  CheckStates��.   Checked��. �� .��W� $ IconRotateTransform��$ Angle�$	 180X�$ 00:00:00D�  	Unchecked��. �� .��W� $ IconRotateTransform��$ Angle�$ 0X�$ 00:00:00D�.+�� $��Stretch=�$��Stretch=�.+��� $��BackgroundBorder��$ 3�����	#FF8A2BE2$��0q�$��Stretch=�$��Stretch=�$ 0Q�F� "����        �    ��[ ���- ��- 	$��Center=�$��Center=�$ 0.5,0.5&�z��� IconRotateTransform��$ 0Q��� " d�.���  �@�Cursor$
 Hand����  � �� �	#FF595959��  � �� �	#FF000000��  � ��$ 1q���  �@d�	IsChecked$
 TrueY���  � �� �� "
 d�.R�� $
��Root��   CommonStates��   $ 0:0:00.2D�$ 	MouseOver��  $ 0:0:00.2D�$ Normal��.   
Normal��  	MouseOver��. �� .��W� $ BackgroundBorder��$ Opacity�$ 
00:00:00.3D�$ 1X�W� $ Arrow��$ StrokeThickness�$ 
00:00:00.3D�$ 1X�  CheckStates��.   Checked��. �� .��W� $ IconRotateTransform��$ Angle�$	 180X�$ 00:00:00D�  	Unchecked��. �� .��W� $ IconRotateTransform��$ Angle�$ 0X�$ 00:00:00D�.+�� $��Stretch=�$��Stretch=�.+��� $��BackgroundBorder��$ 1�����	#FF000000$	��.75q�$��Stretch=�$��12��$��Stretch=�$��12��#��z��$ 1Q�._�F� $��Arrow��=����@B ��[ � 	= �%& ���j ��[ �@B `�� � 	= ��[ ���j `�� 	���	#FFFFFFFF$��Center=�$	��.75��$ 0.5,0.5&�$ 1Q�z��� IconRotateTransform��$	 180Q��� " ��.���  �@�Cursor$
 Hand����  � y�$ Visible=���  � �� �	#FFFFFFFF��  � ��$ Center=���  � ��$ Center=���  �@J�IsHitTestVisible .  ��  �@J�RenderTransformOrigin$ 0.5,0.5&���  � �� �� "
 ��.R�� $
��Root��   CommonStates��   $ 0:0:00.2D�$ 	MouseOver��  $ 0:0:00.2D�$ Normal��.   
Normal��  	MouseOver��. �� .��W� $ BackgroundBorder��$ Opacity�$ 
00:00:00.3D�$ 0X��� $ BackgroundBorder��$ Stroke�$ 
00:00:00.3D�$	 RedX�.+�� $��Stretch=�$��Stretch=�.+��� $��BackgroundBorder��$ 1��$	��0.1q�$
��Left=�$ 1Q� ���� 'F�       " F�.���  � �� �	#FF000000��  � �� �	#FF595959��  � ��$	 .75��._�F� $��border��a����        �     $� �     $� ���� ��� ����� ���� ������@f� ��    	$ 0.5,0.5&�$ 1Q��� #��z $��Center=�8��  �� " d�.���  � �� �	#FFFF0000��  � �� �	#FFFFFFFF��  �@�Cursor$
 Hand����  � y�$ Visible=���  �@d�	IsChecked$ FalseY���  � �� �� "
 d�.R�� $
��Root��   CommonStates��   $ 0:0:00.2D�$ 	MouseOver��  $ 0:0:00.2D�$ Normal��.   
Normal��  	MouseOver��. �� .��W� $ BackgroundBorder��$ Opacity�$ 
00:00:00.3D�$ 1X�W� $ Arrow��$ StrokeThickness�$ 
00:00:00.3D�$ 1X�  CheckStates��.   Checked��. �� .��W� $ IconRotateTransform��$ Angle�$	 180X�$ 00:00:00D�  	Unchecked��. �� .��W� $ IconRotateTransform��$ Angle�$ 0X�$ 00:00:00D�.+�� $��Stretch=�$��Stretch=�.+��� $��BackgroundBorder��$ 1�����	#FFFF0000$��0q�$��Stretch=�$��12��$��Stretch=�$��12��#��z��$ 1Q�._�F� $��Arrow��a������  	= � 	= �� р�[  	= � 	=  	= � 	= �ا ���[  z � z ��� р��  z � z �pr � z �� 	���	#FF808000$	��.75�����	#FFF0FFF0$ 0.5,0.5&�$ 1Q�z��� IconRotateTransform��$ 0Q��� " ��.���  � �� �� "
 ��.R�� $
��Root��$ .5Q�.+��� #��� ��� $��VerticalTemplate��$y�	Collapsed=�$��6���� "  .���  � �� .  ��  �@��PreviewStyle8   ��  �@ VerticalHandleStyle8  ��  �@ HorizontalHandleStyle8  ��  �@ SwitchArrowStyle8  ��  � ��$ Center=���  � �� �� "
  .R�� $
��Root��# z{�   CommonStates��.   
Normal��  	MouseOver��  Disabled��. �� .��W� $
 Root��$ Opacity�$	 0.5X�$ 0D�  FocusStates��   $ 0D�.   	Unfocused��  Focused��. �� .��W� $ FocusVisual��$ Opacity�$ 1X�$ 0D�.+��� $��GridSplitterBackground�����	#FFEFEFF2$��0��� $��HorizontalTemplate�� �� $��5 ��� $��80 ��� $��20 ��� $��80 ��� $	��80* ��� $��14 ��� $��14 ��� $��5 �.+��� 8�� $��1��$ Source��z��� $ 1Q��� 8�� $��3��$
 Data��z��� $ -1Q�$	 -1Q�d� $"��HorizontalGridSplitterHandle��$��5�� . #��z $ 0.5,0.5&�z��� $ -1Q�� $��VerticalTemplate��$y�	Collapsed=� �� $��* ��� $��50 ��� $��* �.+�d� $ ��VerticalGridSplitterHandle��$��1�� . #��z  $ 0.5,0.5&�z��� $	 1Q��� $��FocusVisual�����	#FF000000$��1��$ 0Q� .  