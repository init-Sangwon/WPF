   M S B A M L   `   `   `  ����
  WpfApp    WpfApp.Views.EditWord   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  단어 수정��5   	   $	��200��6   $	��300��6$    � 
ResizeMode$ NoResize=�60    � WindowStartupLocation$ CenterOwner=�6F   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $��* �6   .+�5   
   �� $��영어 단어:��6   $
��Bold"�6"   $��10,5,0,5q�64   �� 5   
   -   5         ��Name$ TxtEng��5      $��0��6"   $��
71,4,10,27q�6/   �� 5   
   $��한글 뜻:��6   $
��Bold"�6!   $��1��63   $��10,5,0,5q�6@   �� 5   
   -   5        $ TxtKor��5      $��1��6"   $��
71,5,10,10q�6/   �� 5   
    �� Orientation$ 
Horizontal=�6   $��2��6.   $��Right=�6;   $��0,10,0,0q�6W   .+�5      �� -   5        $��확인��5      $��70��6"   $��5q�6-   68   �� 5      -   5        $��취소��5      $��70��6"   $��5q�6-   68   5      5      5      