   M S B A M L   `   `   `  ���� S  ORecoderServerApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:RecoderServerApplicationRecoderServerApplication  )    #RecoderServerApplication.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     8local&clr-namespace:RecoderServerApplication       � Title$   Recoder Server Application��$��612.428��$��994.681��.��� .+��� -    ��Name$ startbutton��$��StartServer��$
��Left=�$��
10,525,0,0q�$	��Top=�$��88��$��34���� -    ��Name$ porttext��$
��Left=�$��23��$��
61,489,0,0q� �� TextWrapping$
 Wrap=�$
��8880��$	��Top=�$	��120���� -    ��Name$ label��$��Port：��$
��Left=�$��
10,485,0,0q�$	��Top=��� -    ��Name$ listView��$
��Left=�$	��178��$��
10,302,0,0q�$	��Top=�$	��942�� �� View �� .P��� $��Index�� �� Width$ 80�� �� DisplayMemberBinding �� 	 �� Path$	 Index��� $��
IP Address��$	 110�� �� $	 	IPAddress��� $
��Port��$ 80�� �� $
	 Port��� $��ID��$	 200�� �� $	 ID��� $��State��$	 110�� �� $	 State��� $��Error Package��$	 100�� �� $	 Error��� $��Repair Times��$	 100�� �� $	 RepairTimes�.@��� -   
 ��Name$
 listBox��$	��100��$	��100���� -   $ button1��$��Close��$
��Left=�$��159,525,0,0q�$	��Top=�$��95��$��39���� -   $ fstext��$
��Left=�$��23��$��270,489,0,0q�$
 Wrap=�$
��8000��$	��Top=�$	��120���� -   $ label1��$	��fs:��$
��Left=�$��242,489,0,0q�$	��Top=��� -	   $��scPitch�� � Minimum$
 -180Q� � Maximum$	 180Q�$��1183,-10,348,10q�$y�Hidden=��� -
   $��scYaw��$
 -180Q�$	 180Q�$��1134,-10,397,10q�$y�Hidden=��� -   $��scRoll��$
 -180Q�$	 180Q�$��1161,-10,370,10q�$y�Hidden=��� -   $ OpenCOM��$��OpenCOM��$
��Left=�$��
151,12,0,0q�$	��Top=�$��75���� -   $ bBlink��$��Blink��$
��Left=�$��831,236,0,0q�$	��Top=�$��75��{�.  �� -   $ bInit��$
��Init��$
��Left=�$��831,271,0,0q�$	��Top=�$��75��{�.  �� -   $ bBegin��$��Begin��$
��Left=�$��831,160,0,0q�$	��Top=�$��75��{�.  �� -   $
 bEnd��$	��End��$
��Left=�$��831,197,0,0q�$	��Top=�$��75��{�.  �� -    ��Name$ comboBox��$
��Left=�$��	10,10,0,0q�$	��Top=�$	��120���� -   $ FS��$
��Left=�$��23��$��726,271,0,0q�$
 Wrap=�$
��8000��$	��Top=�$��43���� -   $ GADFS��$
��Left=�$��23��$��774,271,0,0q�$
 Wrap=�$��30��$	��Top=�$��46���� -   $ 	textBox12��$
��Left=�$��23��$��911,271,0,0q�$
 Wrap=�$��4��$	��Top=�$��36���� -   $
��Read��$
��Left=�$��
831,42,0,0q�$	��Top=�$��75���� -   $��	SessionID��$
��Left=�$��23��$��
706,38,0,0q�$
 Wrap=�$��1a2b3c4d5e01010��$	��Top=�$	��120���� $��
SessionID:��$
��Left=�$��
630,36,0,0q�$	��Top=�$��71���� -   $��bindb��$
��Bind��$
��Left=�$��
831,83,0,0q�$	��Top=�$��75��{�.  �� -   $��radiate��{�.  $��Radiate��$
��Left=�$��
246,13,0,0q�$	��Top=�$��75���� -   $
��list��$
��Left=�$	��248��$��	12,37,0,0q�$	��Top=�$	��595�� �� .P��� $��Index��$ 80�� �� $	 Index��� $��Board ID��$	 250�� �� $	 ID��� $
��Bind��$	 250�� �� $
	 Bind��� -   $��wificon��$��Server��$
��Left=�$��831,122,0,0q�$	��Top=�$��75��{�.  �� -   $
��wifi��$
��Left=�$��23��$��706,118,0,0q�$
 Wrap=�$
��ASUS��$	��Top=�$	��120���� -   $
��pass��$
��Left=�$��23��$��706,156,0,0q�$
 Wrap=�$��99775533��$	��Top=�$	��120���� -   $��ip��$
��Left=�$��23��$��706,193,0,0q�$
 Wrap=�$��192.168.50.222��$	��Top=�$	��120���� -   $
��port��$
��Left=�$��23��$��706,232,0,0q�$
 Wrap=�$
��8880��$	��Top=�$	��120���� $��WiFi:��$
��Left=�$��630,116,0,0q�$	��Top=�$��71��$��Right=��� $��	Password:��$
��Left=�$��630,154,0,0q�$	��Top=�$��71��$��Right=��� $��
Server IP:��$
��Left=�$��630,191,0,0q�$	��Top=�$��71��$��Right=��� $��Port:��$
��Left=�$��630,230,0,0q�$	��Top=�$��71��$��Right=��� $��packge:��$
��Left=�$��438,489,0,0q�$	��Top=��� -   $��packge��$
��Left=�$��23��$��497,489,0,0q�$
 Wrap=�$��60��$	��Top=�$	��120���� -    $��calib��$��Calibration��$
��Left=�$��19��$��630,271,0,0q�$	��Top=�$��75���� $��
Server IP:��$
��Left=�$��696,489,0,0q�$	��Top=��� -!   $��serverip��$
��Left=�$��23��$��774,489,0,0q�$
 Wrap=�$��192.168.50.222��$	��Top=�$	��120���� -"   $��bindname��$
��Left=�$��23��$��
706,79,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��120���� -#   $��group��$
��Left=�$��23��$��
831,10,0,0q�$
 Wrap=�$��A��$	��Top=�$��75���� $��Group:��$
��Left=�$��	777,8,0,0q�$	��Top=�