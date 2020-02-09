using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RecoderServerApplication.WAVData;
using RecoderServerApplication.RF24L01;
using RecoderServerApplication.ESP8266;
using System.IO;
using static RecoderServerApplication.ESP8266.Protocol_Keyword_Function;
using RecoderServerApplication.MultiThread;
using System.Timers;
using System.Diagnostics;

namespace RecoderServerApplication
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SoftID = SoftRandom.GetRandomString(8, true, false, true, false, "");
            rand.Content = "ID:" + SoftID;
            Radio_Thread.Elapsed += Radio_Thread_Elapsed;
            UIthread.Start_UI_Refresh(this);
            //Process.Start("speex_decoder.exe","D:\\github_project\\AncientProjects\\RadioRecoder\\WPF_Server\\RecoderServerApplication\\RecoderServerApplication\\bin\\Release\\20190623225152_HS6YX83M\\HQOBJSQZ_2019-06-23-22-52-33-000_REC.wzr");
        }
        RecoderServerApplication.MultiThread.SoftUI_Thread UIthread = new SoftUI_Thread();
        string listbox_value;
        private void Radio_Thread_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                lis.Radio_Send_Message(WIFI_Protocol.Construct_Data_Packet(new TransData_Struct(Protocol_Keyword.State_Binding, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, Encoding.ASCII.GetBytes(listbox_value))));
            }
            catch(Exception e2)
            {

            }
        }

        string SoftID;

        private void Clink(object sender, MouseButtonEventArgs e)
        {
            string[] ArryPort = ComPort.GetALLCOM();
            comboBox.Items.Clear();
            for (int i = 0; i < ArryPort.Length; i++)
            {
                comboBox.Items.Add(ArryPort[i]);
            }
            comboBox.SelectedIndex = 1;
        }

        private void OpenCOM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ComPort.ComisOpen())
                {
                    string strcom = comboBox.Items[comboBox.SelectedIndex].ToString();
                    if (strcom != "")
                    {
                        if (ComPort.OpenCOM(strcom))
                        {
                            OpenCOM.Content = "CloseCOM";
                            comboBox.IsEnabled = false;
                            bBegin.IsEnabled = true;
                            bEnd.IsEnabled = true;
                            bBlink.IsEnabled = true;
                            bInit.IsEnabled = true;
                            radiate.IsEnabled = true;
                            bindb.IsEnabled = true;
                            wificon.IsEnabled = true;
                        }
                        else
                        {
                            MessageBox.Show("Can not Open");
                        }
                    }
                }
                else
                {
                    ComPort.CloseCOM();
                    radiate.IsEnabled = false;
                    OpenCOM.Content = "OpenCOM";
                    comboBox.IsEnabled = true;
                    bBegin.IsEnabled = false;
                    bEnd.IsEnabled = false;
                    bBlink.IsEnabled = false;
                    bInit.IsEnabled = false;

                    bindname.IsEnabled = true;
                    bindb.Content = "Bind";

                    radiate.Content = "Radiate";
                    bindb.IsEnabled = false;
                    wificon.IsEnabled = false;
                    RF_DataAnalysis.Stop_RadioSend();
                }
            }
            catch
            {
                MessageBox.Show("Can not Open");
            }
        }

        private void bBlink_Click(object sender, RoutedEventArgs e)
        {
            RF_DataAnalysis.CastSend_Blink(SoftID);
        }


        private void bBegin_Click(object sender, RoutedEventArgs e)
        {
            RF_DataAnalysis.CastSend_Begin(SoftID);
        }


        private void bEnd_Click(object sender, RoutedEventArgs e)
        {
            RF_DataAnalysis.CastSend_End(SoftID);
        }
        private void bInit_Click(object sender, RoutedEventArgs e)
        {
            RF_DataAnalysis.CastSend_Init(SoftID, FS.Text,GADFS.Text, textBox12.Text);
        }
        //private void BindClink(object sender, MouseButtonEventArgs e)
        //{
        //    string[] ArryPort = { "1", "2", "3", "4", "5", "asdcva" };
        //    bindname.Items.Clear();
        //    for (int i = 0; i < ArryPort.Length; i++)
        //    {
        //        bindname.Items.Add(ArryPort[i]);
        //    }
        //    bindname.SelectedIndex = 0;
        //}

        Timer Radio_Thread = new Timer(100);
        bool Radio_MultiThread = true;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                char[] remove = { ' ','\r','\n' };
                string bind = bindname.Text.Trim(remove);
                if (bind.Length > 10)
                {
                    MessageBox.Show("绑定名称过长");
                    return;
                }
                listbox_value = bindname.Text.Trim(remove).ToString().Trim();
            }
            catch(Exception e2)
            {

            }
            if (Radio_MultiThread)
            {
                Radio_Thread.Start();
                bindb.Content = "Stop";
            }
            else
            {
                Radio_Thread.Stop();
                bindb.Content = "Bind";
            }
            Radio_MultiThread = !Radio_MultiThread;
        }



        bool isradiate = false;
        private void radiate_Click(object sender, RoutedEventArgs e)
        {
            isradiate = !isradiate;
            if (isradiate)
            {
                radiate.Content = "Cancle";
                RF_DataAnalysis.Begin_RadioSend_Search(SoftID);
                calib.IsEnabled = false;
                bindb.IsEnabled = false;
                bBegin.IsEnabled = false;
                bEnd.IsEnabled = false;
                bBlink.IsEnabled = false;
                bInit.IsEnabled = false;
                wificon.IsEnabled = false;
            }
            else
            {
                radiate.Content = "Radiate";
                RF_DataAnalysis.Stop_RadioSend();
                calib.IsEnabled = true;
                bindb.IsEnabled = true;
                bBegin.IsEnabled = true;
                bEnd.IsEnabled = true;
                bBlink.IsEnabled = true;
                bInit.IsEnabled = true;
                wificon.IsEnabled = true;
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            byte[] aaa  = new byte[1024 * 32];
            byte[][] bbb = new byte[Protocol_Keyword_Function.Dic_Protocol.Count][];
            for (int i = 0; i < bbb.Length; i ++)
            {
                bbb[i] = Encoding.ASCII.GetBytes(Dic_Protocol[(Protocol_Keyword)i].State_string);
            }
            Random re = new Random();
            for(int i = 0;i<10000;i++)
                bbb[i % bbb.Length].CopyTo(aaa,re.Next(0,aaa.Length - bbb[i % bbb.Length].Length));
            List<int>[] ddd= WIFI_Protocol.Search_ByteArray_String(ref aaa, ref bbb);
 
            MessageBox.Show(ddd[0].Count.ToString());

        }
        bool iswifibutton = false;
        private void wificon_Click(object sender, RoutedEventArgs e)
        {
            iswifibutton = !iswifibutton;
            if (iswifibutton)
            {
                wificon.Content = "Cancle..";
                calib.IsEnabled = false;
                radiate.IsEnabled = false;
                bindb.IsEnabled = false;
                bBegin.IsEnabled = false;
                bEnd.IsEnabled = false;
                bBlink.IsEnabled = false;
                bInit.IsEnabled = false;
                ip.IsEnabled = false;
                port.IsEnabled = false;
                wifi.IsEnabled = false;
                pass.IsEnabled = false;
                RF_DataAnalysis.Begin_RadioSend_WIFI(SoftID,wifi.Text,pass.Text,ip.Text,port.Text);
            }
            else
            {
                wificon.Content = "Server";
                calib.IsEnabled = true;
                radiate.IsEnabled = true;
                bindb.IsEnabled = true;
                bBegin.IsEnabled = true;
                bEnd.IsEnabled = true;
                bBlink.IsEnabled = true;
                bInit.IsEnabled = true;
                ip.IsEnabled = true;
                port.IsEnabled = true;
                wifi.IsEnabled = true;
                pass.IsEnabled = true;
                RF_DataAnalysis.Stop_RadioSend();
            }
        }
        bool iscalib = false;
        private void calib_Click(object sender, RoutedEventArgs e)
        {
            iscalib = !iscalib;
            if (iscalib)
            {
                calib.Content = "Cancle";
                RF_DataAnalysis.Begin_RadioSend_Calibration(SoftID);
                //calib.IsEnabled = false;
                bindb.IsEnabled = false;
                bBegin.IsEnabled = false;
                bEnd.IsEnabled = false;
                bBlink.IsEnabled = false;
                bInit.IsEnabled = false;
                wificon.IsEnabled = false;
            }
            else
            {
                calib.Content = "Calibration";
                RF_DataAnalysis.Stop_RadioSend();
                //calib.IsEnabled = true;
                bindb.IsEnabled = true;
                bBegin.IsEnabled = true;
                bEnd.IsEnabled = true;
                bBlink.IsEnabled = true;
                bInit.IsEnabled = true;
                wificon.IsEnabled = true;
            }
        }
        MultiThread.ListeningThread lis = new MultiThread.ListeningThread();
        bool isStartServer = false;
        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!isStartServer)
            {
                lis.StartListen(int.Parse(porttext.Text),serverip.Text, int.Parse(fstext.Text), int.Parse(packge.Text), DateTime.Now.ToString("yyyyMMddHHmmss")+"_"+ SoftID);
                startbutton.Content = "StopServer";
            }
            else
            {
                lis.StopListen();
                startbutton.Content = "StartServer";
            }
            isStartServer = !isStartServer;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //byte[] data = { 15, 0, 54, 65, 46, 132, 5, 46, 135, 5, 54, 61, 3, 32, 0, 0, 0, 0x30, 0x31, 0x32,0x33, 0x34, 0x35, 0x0d,0x0a };
            //string str = "012345\r\n";
            //byte[] strdata = Encoding.ASCII.GetBytes(str);
            //int i =WIFI_Protocol.Search_ByteArray_String(ref data,ref strdata);
            //MessageBox.Show(data[i].ToString());
        }
    }
}
