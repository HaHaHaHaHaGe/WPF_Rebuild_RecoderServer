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
            public class BGConvert : IValueConverter
            {

                public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

                {

                    if ((string)value == "在线")

                    {

                        return new SolidColorBrush(Colors.Red);

                    }

                    else

                        return new SolidColorBrush(Colors.White);

                }

                public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

                {

                    throw new NotImplementedException();

                }

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
                for (int i = 0; i < ListeningThread.DeviceList_Thread.Count; i++)
                {
                    ServerListUI data = new ServerListUI
                    {
                        Index = i.ToString(),
                        IPAddress = ListeningThread.DeviceList_Thread[i].ipaddress,
                        Port = ListeningThread.DeviceList_Thread[i].port,
                        ID = ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Device_ID,
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
                                listServerListUI[j].RepairTimes = data.RepairTimes;
                                listServerListUI[j].State = data.State;
                            }
                        }
                        if(flag == false)
                            listServerListUI.Add(data);




                        //item.DataContext = data2;
                        //main.list.Items.Add(data2);
                    });
                }
                Thread.Sleep(100);
            }
        }


    }
}
