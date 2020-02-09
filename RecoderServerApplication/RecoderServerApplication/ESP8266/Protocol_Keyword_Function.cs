using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoderServerApplication.ESP8266
{
    class Protocol_Keyword_Function
    {
        public class TransData_Struct
        {
            public TransData_Struct(Protocol_Keyword Keyword_, byte[] Device_ID_, byte[] Data_)
            {
                Keyword = Keyword_;
                Device_ID = Device_ID_;
                Data = Data_;
            }

            public TransData_Struct() { }
            public Protocol_Keyword Keyword;
            public byte[] Device_ID;
            public byte[] Data;
        }
        public enum Protocol_Keyword
        {
            State_Idle,
            State_Binding,
            State_Bingding_Check,
            State_Binding_Check_Confirm,
            State_TransData,
            State_FixDataRequest,
            State_TransRepairData,
            State_DeviceInfo,
            State_ReadyToStartRecoder
        }
        public class State_Function
        {
            public string State_string;
            public State_Function_Event Function_Event;
            public object State_recv_class;
        }
        public delegate object State_Function_Event(TransData_Struct data);
        public static Dictionary<Protocol_Keyword, State_Function> Dic_Protocol =
        new Dictionary<Protocol_Keyword, State_Function>
        {
            { Protocol_Keyword.State_Idle ,                     new State_Function { State_string = "Device is Idle\r\n",               Function_Event = new State_Function_Event(State_Idle) ,                     State_recv_class = new State_Idle_refData()                     }},
            { Protocol_Keyword.State_Binding ,                  new State_Function { State_string = "Request Binding\r\n",              Function_Event = new State_Function_Event(State_Binding) ,                  State_recv_class = new State_Binding_refData()                  }},
            { Protocol_Keyword.State_Bingding_Check ,           new State_Function { State_string = "Do Bindind Cheking\r\n",           Function_Event = new State_Function_Event(State_Bingding_Check) ,           State_recv_class = new State_Bingding_Check_refData()           }},
            { Protocol_Keyword.State_Binding_Check_Confirm ,    new State_Function { State_string = "Bindind Check Confirm\r\n",        Function_Event = new State_Function_Event(State_Binding_Check_Confirm) ,    State_recv_class = new State_Binding_Check_Confirm_refData()    }},
            { Protocol_Keyword.State_TransData ,                new State_Function { State_string = "Recoder Source Data\r\n",          Function_Event = new State_Function_Event(State_TransData) ,                State_recv_class = new State_TransData_refData()                }},
            { Protocol_Keyword.State_FixDataRequest ,           new State_Function { State_string = "Fix Data Request\r\n",             Function_Event = new State_Function_Event(State_FixDataRequest) ,           State_recv_class = new State_FixDataRequest_refData()           }},
            { Protocol_Keyword.State_TransRepairData ,          new State_Function { State_string = "Recoder Repair Data\r\n",          Function_Event = new State_Function_Event(State_TransRepairData) ,          State_recv_class = new State_TransRepairData_refData()          }},
            { Protocol_Keyword.State_DeviceInfo ,               new State_Function { State_string = "Synchronous Device Info\r\n",      Function_Event = new State_Function_Event(State_DeviceInfo) ,               State_recv_class = new State_DeviceInfo_refData()               }},
            { Protocol_Keyword.State_ReadyToStartRecoder ,      new State_Function { State_string = "Ready To Start Recoder\r\n",       Function_Event = new State_Function_Event(State_ReadyToStartRecoder) ,      State_recv_class = new State_ReadyToStartRecoder_refData()      }}
        };
        public class State_FixDataRequest_refData
        {
            public uint Start;
            public uint End;
        }
        private static object State_FixDataRequest(TransData_Struct data)
        {
            State_FixDataRequest_refData recv = new State_FixDataRequest_refData();
            return recv;
        }
        public class State_Idle_refData
        {
            public byte State;
        }
        private static object State_Idle(TransData_Struct data)
        {
            State_Idle_refData recv = new State_Idle_refData();
            recv.State = data.Data[0];
            return recv;
        }
        public class State_Binding_refData
        {

        }
        private static object State_Binding(TransData_Struct data)
        {
            State_Binding_refData recv = new State_Binding_refData();
            return recv;
        }
        public class State_Bingding_Check_refData
        {
            public string Binding_User;
        }
        private static object State_Bingding_Check(TransData_Struct data)
        {
            State_Bingding_Check_refData recv = new State_Bingding_Check_refData();
            recv.Binding_User = Encoding.ASCII.GetString(data.Data);
            return recv;

        }
        public class State_Binding_Check_Confirm_refData
        {

        }
        private static object State_Binding_Check_Confirm(TransData_Struct data)
        {
            State_Binding_Check_Confirm_refData recv = new State_Binding_Check_Confirm_refData();
            return recv;

        }
        public class State_TransData_refData
        {
            public uint Start_Index;
            public DateTime Init_Time;
            public byte[] wav_data;
        }
        private static object State_TransData(TransData_Struct data)
        {
            State_TransData_refData recv = new State_TransData_refData();
            if (data.Data == null)
                return recv;
            if (data.Data.Length <= 9)
                return recv;
            recv.Start_Index = ((uint)data.Data[0] << 24);
            recv.Start_Index |= ((uint)data.Data[1] << 16);
            recv.Start_Index |= ((uint)data.Data[2] << 8);
            recv.Start_Index |= ((uint)data.Data[3]);

            DateTime time = new DateTime(data.Data[4] + 1980, data.Data[5], data.Data[6], data.Data[7], data.Data[8], data.Data[9]);

            recv.Init_Time = time;

            recv.wav_data = new byte[data.Data.Length - 12];
            Buffer.BlockCopy(data.Data, 12, recv.wav_data, 0, data.Data.Length - 12);
            return recv;
        }
        public class State_TransRepairData_refData
        {
            public uint Repair_Start_Index;
            public DateTime Init_Time;
            public byte[] Repair_wav_data;
        }
        private static object State_TransRepairData(TransData_Struct data)
        {
            State_TransRepairData_refData recv = new State_TransRepairData_refData();

            recv.Repair_Start_Index = ((uint)data.Data[0] << 24);
            recv.Repair_Start_Index |= ((uint)data.Data[1] << 16);
            recv.Repair_Start_Index |= ((uint)data.Data[2] << 8);
            recv.Repair_Start_Index |= ((uint)data.Data[3]);

            DateTime time = new DateTime(data.Data[4] + 1980, data.Data[5], data.Data[6], data.Data[7], data.Data[8], data.Data[9]);

            recv.Init_Time = time;

            recv.Repair_wav_data = new byte[data.Data.Length - 12];
            Buffer.BlockCopy(data.Data, 12, recv.Repair_wav_data, 0, data.Data.Length - 12);
            return recv;
        }
        public class State_DeviceInfo_refData
        {

        }
        private static object State_DeviceInfo(TransData_Struct data)
        {
            State_DeviceInfo_refData recv = new State_DeviceInfo_refData();
            return recv;

        }
        public class State_ReadyToStartRecoder_refData
        {

        }
        private static object State_ReadyToStartRecoder(TransData_Struct data)
        {
            State_ReadyToStartRecoder_refData recv = new State_ReadyToStartRecoder_refData();
            return recv;

        }
    }
}
