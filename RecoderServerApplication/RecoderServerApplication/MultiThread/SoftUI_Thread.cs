using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RecoderServerApplication.MultiThread
{
    class SoftUI_Thread
    {
        Thread Refresh_Thread;
        MainWindow main;
        class ServerListUI
        {
            public string Index { get; set; }
            public string IPAddress { get; set; }
            public string Port { get; set; }
            public string ID { get; set; }
            public string State { get; set; }
            public string Error { get; set; }
            public string RepairTimes { get; set; }
        }
        public class UI_Trans
        {
            public int Index { get; set; }
            public string ID { get; set; }
            public string Bind { get; set; }
        }
        public void Start_UI_Refresh(MainWindow w)
        {
            main = w;
            Refresh_Thread = new Thread(Refresh_Thread_Function);
            Refresh_Thread.Start();
            Refresh_Thread.IsBackground = true;
        }
        void Refresh_Thread_Function()
        {
            
            

            while (true)
            {
                main.Dispatcher.Invoke(() =>
                {
                    main.listView.Items.Clear();
                    main.list.Items.Clear();
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
                        Index = i,
                        ID = ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Device_ID,
                        Bind = ListeningThread.DeviceList_Thread[i].Device_Recv_Struct.Bind_User,
                    };
                    main.Dispatcher.Invoke(() =>
                    {
                        ListViewItem item = new ListViewItem();
                        item.DataContext = data;
                        main.listView.Items.Add(data);
                        item.DataContext = data2;
                        main.list.Items.Add(data2);
                    });
                }
                Thread.Sleep(100);
            }
        }


    }
}
