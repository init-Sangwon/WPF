   M S B A M L   `   `   `  ����F  BAsyncClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:AsyncClientAsyncClient  5   	       AsyncClient.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:AsyncClient     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$	 Log��5      $��0��6   $��0��6,   �� 5   
   $��2��6   $��2��6%   $��Center=�62   $��Center=�6O    �� Orientation$ 
Horizontal=�6j   .+�5      �� -   5        $��Connect��5      $��80��6'   $��20��62   $��10q�6>   6J   �� 5      -   5        $��
DisConnect��5      $��80��6*   $��20��65   6B   5      �� 5   
   $��1��6   $��0��6"   $��Center=�62   $ 
Horizontal=�6M   .+�5      �� -   5        $
 chat��5      $��20q�6%   $	��200��61   �� 5      -   5        $
��Send��5      $��40��6$   $��20��6/   6;   5      5      5      