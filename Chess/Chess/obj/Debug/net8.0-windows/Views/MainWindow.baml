   M S B A M L   `   `   `  ����	  Chess    Chess.Views.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Chess��5   	   $	��600��6   $	��600��6$   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��20(�5      $��10q�5      ��5      �� *CurrentPlayerText+5      �� 5   
   $��1��6   $	��480��5      $	��480��6#   ��5      �� *	Squares+��5      �� 5      .R�5      A�  A� Rows$ 8��6"    A� Columns$ 8��6+   5      ��5      �� 5      .R�5      �� ��6   �� * DataContext.SquareClickCommand+ �� RelativeSource ��  �� AncestorType ItemsControl��5      �� *
Position+��5      �� 5       �� 
TargetType" ��6$   .�5   "   ��  �� Property � �� �� Value 5    )   �� *
Background+	 �� Triggers	 5!   "   �� 5"   &   
 �� Binding
 62   �� *
IsSelected+ �� Value True5#   0   .a�5$   *   ��  � ��
 � ���5%   1   5&   '   5(      .��5*      �� $��Uniform=�5+       ��5*       �� *
PieceImage+5+       5,      5-      5/      50      51      