using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Documents;

namespace RecoderServerApplication.RF24L01
{
    class ComPort
    {
        private static SerialPort scom;
        public static byte[] RecvData = new byte[1024 * 1024];
        private static int RecvDataLoc = 0;

        public static bool ComisOpen()
        {
            try
            {
                return scom.IsOpen;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static string[] GetALLCOM()
        {
            return SerialPort.GetPortNames();
        }

        private static void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[1024];
            int n = scom.Read(data, 0, 1024);
            if (RecvDataLoc + n >= 1024 * 1024)
                RecvDataLoc = 0;
            for (int i = 0; i < n; i++)
            {
                RecvData[RecvDataLoc] = data[i];
                RecvDataLoc++;
            }
        }

        public static bool OpenCOM(string strcom)
        {
            scom = new SerialPort(strcom, 9600);
            scom.DataReceived += Com_DataReceived;
            scom.Open();
            return scom.IsOpen;
        }

        public static void CloseCOM()
        {
            scom.Close();
        }

        public static void ComSendData(string str)
        {


            string data = str;
            Byte head = (byte)(data.Length + 1);
            byte sum = 0;
            byte[] bdata = new byte[data.Length + 2];
            byte[] strdata = Encoding.ASCII.GetBytes(data);
            bdata[0] = head;
            for (int i = 0; i < data.Length; i++)
            {
                bdata[i + 1] = strdata[i];
                sum += strdata[i];
            }
            bdata[data.Length + 1] = sum;
            //string data = "SORID:" + random + "TARID:************" + "BEGIN:Blink";
            scom.Write(bdata, 0, bdata.Length);
        }
        public static void ComSendData(string str,byte[] data_2)
        {


            string data = str;
            Byte head = (byte)(data.Length + data_2.Length + 1);
            byte sum = 0;
            byte[] bdata = new byte[data.Length + 2 + data_2.Length];
            byte[] strdata = Encoding.ASCII.GetBytes(data);
            bdata[0] = head;
            for (int i = 0; i < data.Length; i++)
            {
                bdata[i + 1] = strdata[i];
                sum += strdata[i];
            }
            for (int i = 0; i < data_2.Length; i++)
            {
                bdata[i + 1 + data.Length] = data_2[i];
                sum += data_2[i];
            }
            bdata[data.Length + data_2.Length + 1] = sum;
            //string data = "SORID:" + random + "TARID:************" + "BEGIN:Blink";
            scom.Write(bdata, 0, bdata.Length);
        }
    }
}
