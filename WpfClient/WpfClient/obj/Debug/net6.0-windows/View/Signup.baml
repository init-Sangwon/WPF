   M S B A M L   `   `   `  ����D  @WpfClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:WpfClient.HelpersWpfClient.Helpers  5   	       WpfClient.View.Signup   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3helpersclr-namespace:WpfClient.Helpers     5   	     � Title$  Signup��5   	   $	��400��6   $	��400��6%   .��5      � $��Center=�6   $��Center=�6(    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $
��ID :��6   $��Center=�6   $��14(�6:   $��0��6H   $��0��6U   $��5q�6e   �� 5   
   $��20��6K   $	��120��6W   $��0��6c   $��1��6p   $��5q�6�   ��6   �� *Id+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5   
   $
��PW :��6   $��Center=�6   $��14(�6:   $��1��6H   $��0��6U   $��5q�6e   G� 5   
   )   #WpfClient.Helpers.PasswordBoxHelper   AttachPassword$
 True��5      $��20��5      $	��120��6"   $��1��6.   $��1��6;   $��5q�6K      BoundPassword 5      �� *
Password+ �� Mode$ TwoWay=�5   K   �� 5   
   $��PW 확인 :��6   $��Center=�6"   $��14(�6=   $��2��6K   $��0��6X   $��5q�6h   G� 5   
   $
 True��5       $��20��5!      $	��120��6"   $��2��6.   $��1��6;   $��5q�6K    5      �� *ConfirmPassword+$ TwoWay=�5!   K   �� 5$   
   $��이름 :��6   $��Center=�6   $��14(�6:   $��3��6H   $��0��6U   $��5q�6e   �� 5%   
   $��20��6M   $	��120��6Y   $��3��6e   $��1��6r   $��5q�6�   ��6   �� *Name+$ PropertyChanged=�6�   �� 5(   
   $��4��6   $��2��6"    �� Orientation$ 
Horizontal=�66   $��Right=�6O   $��5q�6k   .+�5)      �� $��가입하기��6   $��75��6F   $��5q�6Q   ��6$   �� *SignupCommand+6Q   5*      5+      5,      