   M S B A M L   `   `   `  ����
  WpfApp    WpfApp.Views.AddWord   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  단어 추가��5   	   $	��220��6   $	��320��6$    � WindowStartupLocation$ CenterScreen=�60    � 
ResizeMode$ NoResize=�6U   $��Centaur)�6k   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $��* �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��영어 단어:��6   $��0��6"   $��0��6/   $��Center=�6?   $��0,5,5,5q�6Z   $
��Bold"�6k   �� 5   
   -   5         ��Name$ 
EngTextBox��5      $��0��6&   $��1��63   $��25��6C   $��Center=�6O   $��0,5,0,5q�6j   �� 5   
   $��한글 뜻:��6   $��1��6!   $��0��6.   $��Center=�6>   $��0,5,5,5q�6Y   $
��Bold"�6j   �� 5   
   -   5        $ 
KorTextBox��5      $��1��6&   $��1��63   $��25��6C   $��Center=�6O   $��0,5,0,5q�6j   �� 5   
   $��2��6   $��2��6"    �� Orientation$ 
Horizontal=�66   $��Right=�6O   $��0,10,0,0q�6k   .+�5      �� $��저장��6   $	��100��6"   $��40��6.   $
��Bold"�6:   
���Gc�� � Cursor$
 Hand��6`   $��Edwardian Script ITC)�6n   ��5      �� 5       �� 
StartPoint$	 0,0&�6*   	 �� EndPoint$		 1,1&�6;   .��5      � 
 � Color$
 Beige��6'    � Offset$	 0.0Q�65   � 5       $
 	#FF5D4F45��6'   $	 2.0Q�69   5!      5#      5$      5%      5&      