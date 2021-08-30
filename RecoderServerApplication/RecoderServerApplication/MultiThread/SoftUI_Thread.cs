﻿using RecoderServerApplication.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RecoderServerApplication.MultiThread
{

    class SoftUI_Thread
    {
        Thread Refresh_Thread;
        MainWindow main;
        public class ServerListUI : INotifyPropertyChanged
        {
            public string index;
            public string Index
            {
                get { return index; }
                set { index = value; OnPropertyChanged(new PropertyChangedEventArgs("Index")); }
            }
            public string ipaddress;
            public string IPAddress
            {
                get { return ipaddress; }
                set { ipaddress = value; OnPropertyChanged(new PropertyChangedEventArgs("IPAddress")); }
            }
            public string port;
            public string Port
            {
                get { return port; }
                set { port = value; OnPropertyChanged(new PropertyChangedEventArgs("Port")); }
            }
            public string id;
            public string ID
            {
                get { return id; }
                set { id = value; OnPropertyChanged(new PropertyChangedEventArgs("ID")); }
            }
            public string name;
            public string Name
            {
                get { return name; }
                set { name = value; OnPropertyChanged(new PropertyChangedEventArgs("Name")); }
            }
            public string state;
            public string State
            {
                get { return state; }
                set { state = value; OnPropertyChanged(new PropertyChangedEventArgs("State")); }
            }
            public string error;
            public string Error
            {
                get { return error; }
                set { error = value; OnPropertyChanged(new PropertyChangedEventArgs("Error")); }
            }
            public string repairtimes;
            public string RepairTimes
            {
                get { return repairtimes; }
                set { repairtimes = value; OnPropertyChanged(new PropertyChangedEventArgs("RepairTimes")); }
            }
            

            #region // INotifyPropertyChanged成员
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, e);
                }
            }
            #endregion
        }
        public class UI_Trans : INotifyPropertyChanged
        {
            public string index;
            public string Index
            {
                get { return index; }
                set { index = value; OnPropertyChanged(new PropertyChangedEventArgs("Index")); }
            }
            public string id;
            public string ID
            {
                get { return id; }
                set { id = value; OnPropertyChanged(new PropertyChangedEventArgs("ID")); }
            }
            public string bind;
            public string Bind
            {
                get { return bind; }
                set { bind = value; OnPropertyChanged(new PropertyChangedEventArgs("bind")); }
            }
            #region // INotifyPropertyChanged成员
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, e);
                }
            }
            #endregion
        }
        BindingList<ServerListUI> listServerListUI = new BindingList<ServerListUI>();
        BindingList<UI_Trans> listUI_Trans = new BindingList<UI_Trans>();
        public void Start_UI_Refresh(MainWindow w)
        {
            main = w;
            Refresh_Thread = new Thread(Refresh_Thread_Function);
            Refresh_Thread.Start();
            Refresh_Thread.IsBackground = true;
            main.listView.Items.Clear();
            main.list.Items.Clear();
            main.listView.ItemsSource = listServerListUI;
            main.list.ItemsSource = listUI_Trans;
        }
        void Refresh_Thread_Function()
        {
            
            

            while (true)
            {
                main.Dispatcher.Invoke(() =>
                {
                    //main.listView.Items.Clear();
                    //main.list.Items.Clear();
                    //listServerListUI.Clear();
                    //listUI_Trans.Clear();
                });
                int count_Online = 0;
                int count_Offline = 0;
                int count_Recording = 0;
                int count_WaitingRecording = 0;

                for (int i = 0; i < ListeningThread.DeviceList_Thread.Count; i++)
                {

                    switch(ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Device_State)
                    {
                        case "离线":
                            count_Offline++;
                            break;
                        case "待机":
                            count_Online++;
                            break;
                        case "录音中":
                            count_Recording++;
                            break;
                        case "等待绑定":
                            break;
                        case "等待录音":
                            count_WaitingRecording++;
                            break;
                        case "检查错误中":
                            break;
                    }



                    string serNickname = "";
                    for (int j = 0; j < SQLite_RW.SqlData.Count; j++)
                    {
                        string serID = SQLite_RW.SqlData[j].DeviceID;
                        
                        if (ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Device_ID == serID)
                        {
                            serNickname = SQLite_RW.SqlData[j].NickName;
                            break;
                        }
                    }
                    ServerListUI data = new ServerListUI
                    {
                        Index = i.ToString(),
                        IPAddress = ListeningThread.DeviceList_Thread[i].ipaddress,
                        Port = ListeningThread.DeviceList_Thread[i].port,
                        ID = ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Device_ID,
                        Name = serNickname,
                        State = ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Device_State,
                        Error = ListeningThread.DeviceList_Thread[i].GetAllErrorNumber().ToString(),
                        RepairTimes = ListeningThread.DeviceList_Thread[i].GetAllRepairNumber().ToString()
                    };
                    UI_Trans data2 = new UI_Trans
                    {
                        Index = i.ToString(),
                        ID = ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Device_ID,
                        Bind = ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Bind_User,
                    };
                    main.Dispatcher.Invoke(() =>
                    {
                        // ListViewItem item = new ListViewItem();
                        // item.DataContext = data;
                        // main.listView.Items.Add(data);
                        bool flag = false;
                        for (int j = 0; j < listUI_Trans.Count; j++)
                        {
                            if (listUI_Trans[j].ID == data2.ID)
                            {
                                flag = true;
                                listUI_Trans[j].Index = data2.index;
                                listUI_Trans[j].Bind = data2.Bind;
                            }
                        }
                        if (flag == false)
                            listUI_Trans.Add(data2);


                        flag = false;
                        for(int j = 0; j < listServerListUI.Count;j++)
                        {
                            if(listServerListUI[j].ID == data.ID)
                            {
                                flag = true;
                                listServerListUI[j].Error = data.Error;
                                listServerListUI[j].Index = data.index;
                                listServerListUI[j].IPAddress = data.IPAddress;
                                listServerListUI[j].Port = data.Port;
                                listServerListUI[j].Name = data.Name;
                                listServerListUI[j].RepairTimes = data.RepairTimes;
                                listServerListUI[j].State = data.State;
                            }
                        }
                        
                        if (flag == false)
                            listServerListUI.Add(data);




                        //item.DataContext = data2;
                        //main.list.Items.Add(data2);
                    });
                }


                main.Dispatcher.Invoke(() =>
                {
                    main.bBegin.IsEnabled = false;
                    main.bInit.IsEnabled = false;
                    main.device_count.Content = ListeningThread.DeviceList_Thread.Count.ToString() + " / " + SQLite_RW.SqlData.Count.ToString();
                    //main.bEnd.IsEnabled = false;
                    if (count_Online == ListeningThread.DeviceList_Thread.Count)
                    {
                        main.bInit.IsEnabled = true;
                    }
                    if (count_WaitingRecording == ListeningThread.DeviceList_Thread.Count)
                    {
                        main.bBegin.IsEnabled = true;
                    }
                });

                Thread.Sleep(100);
            }
        }


    }
}
