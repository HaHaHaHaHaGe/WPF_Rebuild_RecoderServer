using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RecoderServerApplication.ESP8266;

namespace RecoderServerApplication.MultiThread
{
    class ListeningThread
    {
        public static List<ServiceThread> DeviceList_Thread = new List<ServiceThread>();
        Thread ListenThread;
        Socket Listen_Socket;
        IPEndPoint Server_End_Point;
        int _wavfs;
        int _wavfilesec;
        string _wavfiledir;
        bool server_thread = true;
        private void Listen_Function()
        {
            server_thread = true;
            while (server_thread)
            {
                byte[] temporary = new byte[1024 * 10];
                int temporary_len = 0;
                int sendTimes = 0;
                string New_DeviceID;
                List<Protocol_Keyword_Function.TransData_Struct> list = new List<Protocol_Keyword_Function.TransData_Struct>();
                Socket socket;
                try
                {
                    socket = Listen_Socket.Accept();
                }
                catch (Exception e)
                { continue; }
                socket.ReceiveTimeout = 2000;
                while (true)
                {
                    int recv_len = 0;
                    try
                    {
                        byte[] send = WIFI_Protocol.Construct_Data_Packet(new Protocol_Keyword_Function.TransData_Struct(Protocol_Keyword_Function.Protocol_Keyword.State_DeviceInfo,new byte[] {0,0,0,0,0,0,0,0 },new byte[] { }));
                        socket.Send(send);
                        sendTimes++;
                        recv_len = socket.Receive(temporary, temporary_len, 1024 * 10 - temporary_len, SocketFlags.None);

                    }
                    catch(Exception e)
                    {
                        if (sendTimes == 5)
                        {
                            socket.Close();
                            break;
                        }
                        continue;
                    }
                    if (recv_len > 0)
                    {
                        WIFI_Protocol.AnalysisRawData(ref temporary, ref list);
                        if (list.Count > 0)
                        {
                            New_DeviceID = Encoding.ASCII.GetString(list[0].Device_ID);
                            
                            int i;
                            for (i = 0; i < DeviceList_Thread.Count; i++)
                            {
                                if (DeviceList_Thread[i].Device_Recv_Struct.Device_ID == New_DeviceID)
                                {
                                    DeviceList_Thread[i].Ser = socket;
                                    break;
                                }
                            }
                            if (i == DeviceList_Thread.Count)
                            {
                                ServiceThread server = new ServiceThread(_wavfs, _wavfilesec, _wavfiledir, socket);
                                string str = socket.RemoteEndPoint.ToString();
                                int con = 0;
                                for (int j = 0; j < str.Length; j++)
                                {
                                    if (str[j] == ':')
                                        con = j;
                                }
                                string port = str.Substring(con + 1);
                                string ip = str.Substring(0, con);
                                server.Device_Recv_Struct.Device_ID = New_DeviceID;
                                server.ipaddress = ip;
                                server.port = port;
                                DeviceList_Thread.Add(server);
                            }
                            break;
                        }
                        else
                        {
                            temporary_len += recv_len;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }


        public void StartListen(int port,string ip,int wavfs,int wavfilesec,string wavfiledir)
        {
            _wavfs = wavfs;
            _wavfilesec = wavfilesec;
            _wavfiledir = wavfiledir;
            //Server_End_Point = new IPEndPoint(IPAddress.Parse(GetIpAddress()), port);
            Server_End_Point = new IPEndPoint(IPAddress.Parse(ip), port);
            Listen_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Listen_Socket.Bind(Server_End_Point);
            Listen_Socket.Listen(0);

            ListenThread = new Thread(Listen_Function);
            ListenThread.Start();
            ListenThread.IsBackground = true;
        }
        public void StopListen()
        {
            Listen_Socket.Dispose();
            Listen_Socket.Close();
            server_thread = false;
            foreach (var item in DeviceList_Thread)
            {
                item.CloseThread();
            }
            DeviceList_Thread.Clear();
        }
        private string GetIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
                                                                    //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];

            return localaddr.ToString();
        }

        public void Radio_Send_Message(byte[] data)
        {
            foreach (var item in DeviceList_Thread)
            {
                try
                {
                    item.SendData(data);
                }
                catch(Exception e)
                {

                }
            }
        }
    }
}
