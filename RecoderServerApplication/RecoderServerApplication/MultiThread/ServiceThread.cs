using RecoderServerApplication.SQLite;
using RecoderServerApplication.Tools;
using RecoderServerApplication.WAVData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RecoderServerApplication.ESP8266.Protocol_Keyword_Function;
using static RecoderServerApplication.ESP8266.WIFI_Protocol;
using static RecoderServerApplication.WAVData.SrcDataCutApart;


namespace RecoderServerApplication.MultiThread
{
    class ServiceThread
    {
        byte[] RecvData = new byte[1024 * 32];
        int RecvData_Location = 0;
        SrcDataCutApart WavCreate = null;
        public Socket Ser = null;
        public int Fre;
        public int Sec;
        public string Dir;
        public string ipaddress;
        public string port;
        Thread Service_Function;
        bool isEnable_Timer = false;
        public int GetAllErrorNumber()
        {
            if (WavCreate == null)
                return 0;
            return WavCreate.GetAllWav_ERROR_Number();
        }
        public int GetAllRepairNumber()
        {
            if (WavCreate == null)
                return 0;
            return WavCreate.GetAllWav_Repair_Number();
        }

        public ServiceThread(int F,int S,string D,Socket tar)
        {
            Fre = F;
            Sec = S;
            Dir = D;
            Ser = tar;
            Service_Function = new Thread(ServiceFunction);
            Service_Function.Start();
            Service_Function.IsBackground = true;
        }
        public void updateRecoderDir(string D)
        {
            Dir = D;
        }
        public void CloseThread()
        {
            Ser.Close();
            Thread_Loop = false;
        }

        public int SendData(byte[] data)
        {
            try
            {
                if (Thread_Loop)
                    return Ser.Send(data);
                else
                    return -1;
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        public class Device_Info
        {
            public string Device_ID;
            public string Bind_User;
            public string Device_State;
            public List<Error_Statistical> Error;
        }
        public Device_Info Device_Recv_Struct = new Device_Info();
        enum Device_State
        {
            Offline,
            Online,
            WaitingBinding,
            WaitingRecording,
            Recording,
            ErrorCorrection
        }
        Device_State dstate;
        bool Thread_Loop = true;

        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            dstate = Device_State.Offline;
            Device_Recv_Struct.Device_State = "离线";
            isEnable_Timer = false;
        }
        public void ServiceFunction()
        {
            
            dstate = Device_State.Online;
            Device_Recv_Struct.Device_State = "待机";
            System.Timers.Timer t = new System.Timers.Timer(60000);//实例化Timer类，设置间隔时间为10000毫秒；

            t.Elapsed += new System.Timers.ElapsedEventHandler(theout);//到达时间的时候执行事件；

            t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；

            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            t.Stop();
            while (Thread_Loop)
            {
                try
                {
                    if (RecvData_Location > RecvData.Length - 500)
                        RecvData_Location = 0;
                    int recv = Ser.Receive(RecvData, RecvData_Location, RecvData.Length - RecvData_Location, SocketFlags.None);
                    if (recv > 0)
                        RecvData_Location += recv;
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                    if(dstate == Device_State.Recording && !isEnable_Timer)
                    {
                        isEnable_Timer = true;
                        t.Start();
                        continue;
                    }
                    dstate = Device_State.Offline;
                    Device_Recv_Struct.Device_State = "离线";
                    continue;
                }
                List<TransData_Struct> lis = new List<TransData_Struct>();
                AnalysisRawData(ref RecvData, ref lis);
                t.Stop();
                isEnable_Timer = false;
                foreach (var item in lis)
                {
                    object recv = State_Response_Function(item);

                    //State_DeviceInfo_refData info = SqlData as State_DeviceInfo_refData;
                    //State_ReadyToStartRecoder_refData ready = SqlData as State_ReadyToStartRecoder_refData;
                    //State_TransData_refData tdata = SqlData as State_TransData_refData;
                    //State_TransRepairData_refData rdata = SqlData as State_TransRepairData_refData;

                        //Device_Recv_Struct.Device_ID = Encoding.ASCII.GetString(item.Device_ID);

                    if (recv is State_Idle_refData idle_refdata)
                    {
                        dstate = (Device_State)idle_refdata.State;
                        Device_Recv_Struct.Bind_User = idle_refdata.Binding_User;
                        switch (dstate)
                        {
                            case Device_State.Online:
                                Device_Recv_Struct.Device_State = "待机";
                                if(WavCreate != null)
                                {
                                    WavCreate.CloseAllWavFile();
                                    WavCreate = null;
                                }
                                break;
                            case Device_State.Recording:
                                Device_Recv_Struct.Device_State = "录音中";
                                break;
                            case Device_State.WaitingBinding:
                                Device_Recv_Struct.Device_State = "等待绑定";
                                break;
                            case Device_State.WaitingRecording:
                                Device_Recv_Struct.Device_State = "等待录音";
                                break;
                            case Device_State.ErrorCorrection:
                                Device_Recv_Struct.Device_State = "检查错误中";
                                break;
                        }
                    }
                    if(recv is State_Bingding_Check_refData bind_refdata)
                    {
                        Device_Recv_Struct.Bind_User = bind_refdata.Binding_User;
                        byte[] confirm = Construct_Data_Packet(new TransData_Struct(Protocol_Keyword.State_Binding_Check_Confirm,item.Device_ID,new byte[] { }));
                        SendData(confirm);
                    }
                    if(recv is State_ReadyToStartRecoder_refData ready_refdata)
                    {
                        Device_Recv_Struct.Device_State = "等待录音";
                        dstate = Device_State.WaitingRecording;
                        if (WavCreate == null)
                        {
                            // Directory.CreateDirectory(Dir +"\\"+ Device_Recv_Struct.Device_ID);
                            Directory.CreateDirectory(Dir);
                            string userid = "";
                            List<SQLite_DataStruct> sqldata = SQLite_RW.GetData();
                            for (int i = 0; i< sqldata.Count;i++)
                            {
                                if(sqldata[i].DeviceID == Device_Recv_Struct.Device_ID)
                                {
                                    userid = sqldata[i].UserID;
                                    break;
                                }
                            }
                            WavCreate = new SrcDataCutApart(Fre, Sec, Dir + "\\" + userid + "_" + Device_Recv_Struct.Device_ID + "_");
                        }
                    }
                    if(recv is State_TransData_refData trans_refdata)
                    {
                        Device_Recv_Struct.Device_State = "录音中";
                        dstate = Device_State.Recording;
                        if(WavCreate == null)
                        {
                            string userid = "";
                            List<SQLite_DataStruct> sqldata = SQLite_RW.GetData();
                            for (int i = 0; i < sqldata.Count; i++)
                            {
                                if (sqldata[i].DeviceID == Device_Recv_Struct.Device_ID)
                                {
                                    userid = sqldata[i].UserID;
                                    break;
                                }
                            }
                            WavCreate = new SrcDataCutApart(Fre, Sec, DataRecovery.getLastAudioFolder() + "\\" + userid + "_" + Device_Recv_Struct.Device_ID + "_");
                            WavCreate.InitNopList(DataRecovery.getLastRecoderProgress(DataRecovery.getLastAudioFolder(), Device_Recv_Struct.Device_ID));
                        }
                        if (WavCreate != null && trans_refdata.wav_data != null)
                        {
                            WavCreate.WriteWavData(trans_refdata.wav_data, trans_refdata.Start_Index, trans_refdata.Init_Time);
                            Device_Recv_Struct.Error = WavCreate.DetailedErrorData();
                        }
                    }
                    if(recv is State_TransRepairData_refData repair_refdata)
                    {
                        if (WavCreate != null)
                        {
                            WavCreate.WriteWavData(repair_refdata.Repair_wav_data, repair_refdata.Repair_Start_Index, repair_refdata.Init_Time);
                            Device_Recv_Struct.Error = WavCreate.DetailedErrorData();
                        }
                    }
                    if (dstate == Device_State.Recording || dstate == Device_State.ErrorCorrection)
                    {

                        List<Error_Statistical> list_error = Device_Recv_Struct.Error;
                        if (list_error == null || WavCreate == null)
                            continue;
                        if (dstate == Device_State.ErrorCorrection && WavCreate.GetAllWav_ERROR_Number() == 0)
                        {
                            byte[] recvdata = Construct_Data_Packet(new TransData_Struct(Protocol_Keyword.State_Idle, item.Device_ID, new byte[] { }));
                            SendData(recvdata);
                        }
                        for (int i = 0; i < list_error.Count; i++)
                        {
                            bool isloop = false;
                            for (int j = 0; j < list_error[i].errorlist.Count; j++)
                            {
                                uint start = list_error[i].errorlist[j].start_location;
                                uint end = list_error[i].errorlist[j].end_location;
                                byte[] confirm = Construct_Data_Packet(new TransData_Struct(Protocol_Keyword.State_FixDataRequest, item.Device_ID,
                                    new byte[]
                                    {
                                    (byte)(start >> 24),
                                    (byte)(start >> 16),
                                    (byte)(start >> 8),
                                    (byte)(start),
                                    (byte)(end >> 24),
                                    (byte)(end >> 16),
                                    (byte)(end >> 8),
                                    (byte)(end),
                                    }));
                                SendData(confirm);
                                isloop = true;
                                break;
                            }
                            if (isloop)
                                break;
                        }
                    }

                }
            }
        }
    }
}
