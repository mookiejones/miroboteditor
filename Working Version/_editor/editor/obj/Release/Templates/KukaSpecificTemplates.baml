   M S B A M L   `   `   `  ���� L  HmiRobotEditor, Version=0.0.0.21359, Culture=neutral, PublicKeyToken=nullA%clr-namespace:miRobotEditor.ViewModelmiRobotEditor.ViewModel  A%clr-namespace:miRobotEditor.LanguagesmiRobotEditor.Languages  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� DataType6    0miRobotEditor.ViewModel.SystemFunctionsViewModel �� Orientation � 	Resources �� 
TargetType �� Property �� Value d� 	IsChecked,   &miRobotEditor.Languages.DatCleanHelper   Key � ColumnDefinitions � RowDefinitions	 �� TextAlignment
 �� SelectedIndex �� View �� CellTemplate �� NotifyOnSourceUpdated �� Width � Value�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     ,vm%clr-namespace:miRobotEditor.ViewModel  .lang%clr-namespace:miRobotEditor.Languages  %  (��       *,  +)(�� �    *, +)�� "    .R��� $ Vertical=� ���� '��       " ��.���  � ��$
 Left=���  � ��$ 	10,10,0,0q�.+��� $��Programs�� �� *
Programs+�� $��	Functions�� �� *	Functions+�� $��
Structures�� �� *
Structures+�� $��	Variables�� �� *	Variables+�� $ 
Horizontal=�.+��� $��_Cancel��$
��Left=�$
��10,5q�$��75���� $	��_Ok��$
��Left=�$
��10,5q�$��75���� "   .R�� ���� *Width+���� *Height+ ���� ButtonStyle  " ��.���  � ��$ 25�� �� $	��350 ��� $
��180* � �� $��0.736501442606485* ��� $��0.683533706778401* ��� $��35 �.+��� $��	Variables��$��2��$	��250��$��5q�.��� $��1�� �� $��35 ��� $��* ��� $��30 � �� $��2.03292072663583* ��� $��1.64225132846164* ��� $��0.666105973025671* �.+��� $ ��List unused variables from��$	 Center=�$��Center=�$��0���� $��1��$��2��$��25��$��0��
 �� *SelectedVarIndex+���� *UsedVarTypes+�� $��ListViewVar��$��0��$��3��$��1������ *	ListItems+ �� .P��� $
��Name�� �� .R��� .+��� $��93��$��Center=����� *Name+ �� *
IsSelected+ . �� $
��Type�� �� .R��� .+��� $��50��$��Center=����� *Type+�� $��Value�� �� .R��� .+��� $��Center=����� *Value+�� $��Declaration��$
 Auto�� �� .R��� .+��� $��Center=����� *Path+�� $ 
Horizontal=�$��2��$��3��.+��� $��25��$��
Select All��$��5,0,0,0q�$	��110��$
��Left=����� *SelectAllCommand+�� $��25��$��Invert Selection��$��5,0,5,0q�$	��110������ *InvertSelectionCommand+�� $��2��$��Center=�$��2������ *NumberSelected+ . �� $��Variable Types��$��0��$��1��$��Stretch=�$��Stretch=�$��0,0,0,0q�$	��NaN��$	��Top=�.��� $	��242��$��196.04�� �� $	��100 ��� $��* � �� $��25 ��� $��25 ��� $��35 ��� $��* ��� $��* �.+�	� $��0��$��0��$��Ignore types��$��0,0,0,0q� �� *IgnoreTypes+	� $��0��$��1��$��Exclusive types��$��0,0,0,0q� �� *ExclusiveTypes+�� $��0��$��25��$��5,0,0,0q�$��2���� $��0��$��3��$��2��$��5q�$��Stretch=�$��Stretch=�$	��100��$	��100���� $��5q�$��1��$��2��$	��Add��$��65��$
��Left=����� *AddVarTypeCmd+�� $��5q�$��1��$��3��$��25��$��Delete��$��65��$
��Left=����� *DeleteVarTypeCmd+�� $��1��$��5��$��25��$��Clear��$��5q�$
��Left=�$��65���� $��85��$��Options��$��1��$��5q�$��1��$	��Top=�.���  �� $��* ��� $��* �.+�	� $��0��$��Delete declaration�� �� *DeleteDeclaration+	� $��1��$��Comment Out Declaration�� �� *CommentDeclaration+� $��2��$	��Top=�$��0��$��5q�$��25��$
��Auto�� �� *
Progress+�� $��2��$��25��$	��Top=�$��1��$
��Left=�$��Execute��$��65��$��50,0,0,0q��� $��2��$��1��$	��Top=�$
��Left=�$��25��$��65��$��Close��$��	125,0,0,0q�