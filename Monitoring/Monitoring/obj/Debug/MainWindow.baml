   M S B A M L   `   `   `  ����  
Monitoring    Monitoring.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  
Monitoring��5   	   $	��800��6   $
��1400��6)    � 	Resources 5         HeaderStyle �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� Property �� Value  RedBrush �� Color  SolidBorderBrush  
GreenBrush  DisabledBackgroundBrush  DisabledBorderBrush  DisabledForeGroudBrush �� 
TargetType �� CornerRadius ��Name	 �� Triggers
 P� Property P� Value �� 
TargetName��%R  &	        &	 �     &	     &	 1    &	 P    &	 n    &	 �    '�� �    7  7  7  7  7  7  �� 5
   
   "  6$   .�5      ��  � ��
 �����6+   �� 5       � ��$ 1q�60   �� 5       � ��
 �   �6,   �� 5       �@J�SnapsToDevicePixels . 64   �� 5       �@�Cursor$
 Hand��6'   5      �� 5   
   $	 Red��6+   �� 5   
   $
 #888��63   �� 5   
   $ Green��6-   �� 5   
   $
 #EEE��6:   �� 5   
   $
 #AAA��66   �� 5   
   $
 #888��69   �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      tabController��" ��6=   .R�5      � .+�5       �� $��colorBorder��6%   $��0,0,-4,0q�5!   %   8��  8�� $��1,1,1,1q�5$   %   $ 0,0,0,0��5%   %   ._�5'   "   �� $ ContentSite��63   $��Center=�6H   $��Center=�6c   $��Header��5(   3   $��	12,2,12,2q�6J   ��. 5)   3   5*      5+      	 5-      P� 5.      
 �@��
IsSelected . . �5/   "   ��  �@K�ZIndex$	 100��6A   �� 50   "   $ colorBorder��6?   8   � ��6X   �� 51   "   $ colorBorder��6D    � ��$ 1,1,1,0q�6]   52      P� 54      
 � {� .  . �55   "   �� $ colorBorder��6?   8   � ��6X   �� 56   "   $ colorBorder��6@   8   � ��6Y   �� 57   "   8   � ��6?   58      5<      5>      5?      5@      .��5B      � -   5         �Name$ mainGrid��5B      .+�5C   
   �� -   5         ��Name$ mainTab��5C      $
��Left=�6&   $	��800��6A   $	��Top=�5D      $
��1400��6-   .@�5E      �� -   5        $��
header_Two��5E      $��Second��6(   .��5F      � -   5        $ 	innerGrid��5F       � ColumnDefinitions 5G      �� 5H      $	��25* �6+   �� 5I      $
��187* �6+   �� 5J      $��1278* �6+   .+�5L      &   System.Windows.Controls.DataGrid  -   5          Name$ listView��5L      $
��Left=�61   $��-6,0,-4,-34q�6L   $	��Top=�5M      $	��800��67   $
��1400��6D      	RowHeight$ 40Q�6Q      
IsReadOnly . "   VerticalScrollBarVisibility$
 Auto=�5N         AutoGenerateColumns .  $��3��6^      ColumnHeaderStyle# [  5O         Columns 5P      0  *System.Windows.Controls.DataGridTextColumn  5Q      ,  &System.Windows.Controls.DataGridColumn   Header$ TARGET_TABLE_NAME��61      
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 6L   �� *Target_table_name+6r     5R      $ ETL_PROCESS_SEG��61    .  6J   �� *Etl_process_seg+6n     5S      $ 
START_DATE��61    .  6E   �� *
Start_date+6d     5T      $ FINISH_DATE��61    .  6F   �� *Finish_date+6f     5U      $ ERROR_STATUS��61    .  6G   �� *Error_status+6h     5V      $ ERROR_MESSAGE��61    .  6H   �� *Error_Message+6j   5X      5Y      5Z      5[      5\      5]      