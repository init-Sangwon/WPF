   M S B A M L   `   `   `  ����
  WpfApp    WpfApp.Views.Memorization   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Memorization��5   	   $	��300��6   $	��400��6+    � WindowStartupLocation$ CenterScreen=�5   	   ���#F9F9F9$��Malgun Gothic)�5      .��5      � $��10q�6    � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� 
���z��� �� CornerRadius$ 10��6)    �� Padding$ 10q�6;   ._�5      �� -   5         ��Name$ WordTextBlock��5      $��26(�5      $
��Bold"�5      $��Center=�5      $��Center=�5      
�������$��영어 단어��5      5      �� 5   
   -   5         ��Name$ MeaningListBox��5      $��1��6*   $
��0,10q�67    �� SelectionMode$ Single=�6E   $��0q�5      
���������5      �� 5      .R�5      �� ���#E6F1FA$ 5��62   $	��0,5q�6C   $ 8q�6P   ._�5       �� $��Center=�5!   $   $��16(�5"   $   ���#333333 � Cursor$
 Hand��5$   $   ��5    $   �� 5$   $   5%      5&      5(      �� 5+   
   -   5        $��정답 확인��5+      $��2��5,      $	��150��5-      $��40��5.      
�������
�������$��16(�51      $
��Bold"�52      $
 Hand��53      $��0q�54      $��Center=�55      $
��0,10q�56      ��58      �� 59      	 �� 
TargetType"	 ��6   .�5:      �� 
 �� Property
 � �� �� Value 5;      �� 5<       �� 
TargetType" ��6.   .R�5=   "   �� #��z��6)   $ 20��5>   )   #��z��5?   )   #��z��5@   )   ._�5A   &   �� $��Center=�67   $��Center=�5B   7   5C   #   5D      5F       �� Triggers 5G      P� 5H       P� Property �@J�IsMouseOver P� Value$
 True��69   . �5I      �� 
 � �� �#45A0496;   5J      P� 5K       � ��$
 True��67   . �5L      �� 
 � �� �#388E3C6;   5M      5O      5Q      5R      5S      