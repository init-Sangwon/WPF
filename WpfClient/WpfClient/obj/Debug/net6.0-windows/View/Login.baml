   M S B A M L   `   `   `  ����D  @WpfClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:WpfClient.HelpersWpfClient.Helpers  5   	   /clr-namespace:WpfClient.ViewWpfClient.View  5   	       WpfClient.View.Login   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   3helpersclr-namespace:WpfClient.Helpers     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:WpfClient.View     5   	     � Title$  Login��5	   	   $	��400��6   $	��400��6$   .��5      � $��Center=�6   $��Center=�6(    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��ID  :��6   $��Center=�6    $��40��6;   $��14(�6F   $��0��6T   $��0��6a   $��5q�6q   �� 5   
   $��20��6K   $	��120��6W   $��0��6c   $��1��6p   $��5q�6�   ��6   �� *Id+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5   
   $
��PW :��6   $��Center=�6   $��40��6:   $��14(�6E   $��1��6S   $��0��6`   $��5q�6p   G� 5   
   )   #WpfClient.Helpers.PasswordBoxHelper   AttachPassword$
 True��6`   $��20��6�   $	��120��6�   $��1��6�   $��1��6�   $��5q�6�      BoundPassword 6   �� *
Password+ �� Mode$ TwoWay=�6�   �� 5!   
   $��2��6   $��2��6"    �� Orientation$ 
Horizontal=�66   $��Right=�6O   $��5q�6k   .+�5"      �� $��	로그인��6      BasicButtonStyle#��[  6#   $��75��6M   $��5q�6X   ��6c   �� *LoginCommand+�� 5#      -   5        $��회원가입��5#      #��[  6$   $��75��6N   $��5q�6Y   6d   5$      5%      5'      