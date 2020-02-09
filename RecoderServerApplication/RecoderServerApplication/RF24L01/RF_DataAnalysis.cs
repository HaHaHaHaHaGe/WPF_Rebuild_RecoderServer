using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace RecoderServerApplication.RF24L01
{
    class RF_DataAnalysis
    {
        private static System.Timers.Timer sendtimer;

        public static void CastSend_Blink(string softID)
        {
            ComPort.ComSendData(softID + "BEGIN:Blink");
        }

        public static void CastSend_Begin(string softID)
        {
            ComPort.ComSendData(softID + "BEGIN:Begin");
        }

        public static void CastSend_End(string softID)
        {
            ComPort.ComSendData(softID + "BEGIN:End");
        }



        public static bool CastSend_Init(string softID, string fs ,string gadfs , string amvl)
        {
            try
            {
                int _FS, _GADFS, AM;
                _FS = int.Parse(fs);
                _GADFS = _FS / (int.Parse(gadfs) * 512);
                AM = int.Parse(amvl);

                if (_FS > 32000 || _FS < 8000)
                {
                    return false;
                }

                if (_GADFS == 0)
                    _GADFS = 1;
                Byte head = (Byte)(10 + fs.Length + _GADFS.ToString().Length);
                string data = softID + "BEGIN:Init" + _FS.ToString() + "-" + _GADFS.ToString() + "-" + AM.ToString();
                ComPort.ComSendData(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Begin_RadioSend_WIFI(string softID, string w_name,string w_pass , string s_ip, string s_port)
        {
            sendtimer = new System.Timers.Timer(1000);
            sendtimer.Elapsed += new ElapsedEventHandler((s, e) => Sendtimer_Elapsed_WiFi(s,e, softID, w_name, w_pass, s_ip, s_port)); 
            sendtimer.Start();
        }

        public static void Begin_RadioSend_Search(string softID)
        {
            sendtimer = new System.Timers.Timer(1000);
            sendtimer.Elapsed += new ElapsedEventHandler((s, e) => Sendtimer_Elapsed_Search(s, e, softID));
            sendtimer.Start();
        }

        public static void Begin_RadioSend_Calibration(string softID)
        {
            sendtimer = new System.Timers.Timer(100);
            sendtimer.Elapsed += new ElapsedEventHandler((s, e) => Sendtimer_Elapsed_Calibration(s, e, softID));
            sendtimer.Start();
        }

        public static void Stop_RadioSend()
        {
            try
            {
                sendtimer.Stop();
                sendtimer.Close();
            }
            catch(Exception e)
            {

            }
        }
        private static void Sendtimer_Elapsed_Calibration(object sender, ElapsedEventArgs e, string softID)
        {
            DateTime now = DateTime.Now;
            byte year = (byte)(now.Year - 1980);
            byte month = (byte)now.Month;
            byte day = (byte)now.Day;
            byte hour = (byte)now.Hour;
            byte minute = (byte)now.Minute;
            byte second = (byte)now.Second;
            Int16 millise = (Int16)now.Millisecond;

            byte[] time_a = new byte[8];


            time_a[0] = year;
            time_a[1] = month;
            time_a[2] = day;
            time_a[3] = hour;
            time_a[4] = minute;
            time_a[5] = second;
            time_a[6] = (byte)(millise >> 8);
            time_a[7] = (byte)millise;



            ComPort.ComSendData("CALIB" + softID , time_a);
        }


        private static void Sendtimer_Elapsed_Search(object sender, ElapsedEventArgs e, string softID)
        {
            ComPort.ComSendData("SERCH:Begin_" + softID);
        }

        private static void Sendtimer_Elapsed_WiFi(object sender, ElapsedEventArgs e, string softID, string w_name, string w_pass, string s_ip, string s_port)
        {
            string data1 = softID + "$WIFI" + w_name + "\n";
            string data2 = softID + "$PASS" + w_pass + "\n";
            string data3 = softID + "$IPAD" + s_ip + "\n";
            string data4 = softID + "$PORT" + s_port + "\n";

            ComPort.ComSendData(data1);
            Thread.Sleep(50);
            ComPort.ComSendData(data2);
            Thread.Sleep(50);
            ComPort.ComSendData(data3);
            Thread.Sleep(50);
            ComPort.ComSendData(data4);
            Thread.Sleep(50);
        }

    }
}
