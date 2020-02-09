using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RecoderServerApplication.ESP8266.Protocol_Keyword_Function;

namespace RecoderServerApplication.ESP8266
{
    class WIFI_Protocol
    {
        public static int Search_ByteArray_String(ref byte[] srcdata, ref byte[] data ,ref bool is_only_one)
        {
            int j = 0;
            bool search = false;
            int search_loc = 0;
            is_only_one = true;
            for (int i = 0; i < srcdata.Length;i ++)
            {
                if (srcdata[i] == data[j])
                    j++;
                else
                    j = 0;
                if (j == data.Length)
                {
                    if (search)
                    {
                        is_only_one = false;
                        return search_loc;
                    }
                    else
                    {
                        search = true;
                        search_loc = i - j + 1;
                        j = 0;
                    }
                }
            }
            if (search)
                return search_loc;
            return -1;
        }

        public static List<int>[] Search_ByteArray_String(ref byte[] srcdata, ref byte[][] data)
        {
            int[] search_len = new int[data.Length];
            List<int>[] return_value = new List<int>[data.Length];
            for (int i = 0; i < data.Length; i++)
                return_value[i]= new List<int>();

            for (int i = 0; i < srcdata.Length; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    if (srcdata[i] == data[j][search_len[j]])
                        search_len[j]++;
                    else
                        search_len[j] = 0;
                    if (search_len[j] + 1 == data[j].Length)
                    {
                        return_value[j].Add(i - search_len[j] + 1);
                        search_len[j] = 0;
                    }
                }
            }
            return return_value;
        }
        public static void AnalysisRawData(ref byte[] srcdata, ref List<TransData_Struct> RecvArray)
        {
            bool isSearch = true;
            while (isSearch)
            {
                //string str = System.Text.Encoding.ASCII.GetString(srcdata);
                //str = str.Replace('\0', '*');
                isSearch = false;


                byte[][] search_list = new byte[Dic_Protocol.Count][];
                for (int i = 0; i < search_list.Length; i++)
                    search_list[i] = Encoding.ASCII.GetBytes(Dic_Protocol[(Protocol_Keyword)i].State_string);

                List<int>[] recvfindlist = Search_ByteArray_String(ref srcdata, ref search_list);

                for (int i = 0; i < recvfindlist.Length; i++) 
                {
                    for (int j = 0; j < recvfindlist[i].Count; j++)
                    {
                        int commLoc = recvfindlist[i][j];
                        if ((commLoc + search_list[i].Length + 4 + 4) >= srcdata.Length)
                        {
                            for (int k = 0; k < search_list[i].Length; k++)
                                srcdata[commLoc + k] = 0;
                            break;
                        }
                        int recvdatalen = srcdata[commLoc + search_list[i].Length] << 24;
                        recvdatalen |= srcdata[commLoc + search_list[i].Length + 1] << 16;
                        recvdatalen |= srcdata[commLoc + search_list[i].Length + 2] << 8;
                        recvdatalen |= srcdata[commLoc + search_list[i].Length + 3];
                        int recvdatacheck = srcdata[commLoc + search_list[i].Length + 4] << 24;
                        recvdatacheck |= srcdata[commLoc + search_list[i].Length + 4 + 1] << 16;
                        recvdatacheck |= srcdata[commLoc + search_list[i].Length + 4 + 2] << 8;
                        recvdatacheck |= srcdata[commLoc + search_list[i].Length + 4 + 3];
                        int recvsum = 0;
                        for (int k = 0; k < search_list[i].Length; k++)
                            recvsum += search_list[i][k];
                        if (recvdatalen > 1024 * 20 || (commLoc + search_list[i].Length + 4 + 4 + recvdatalen + 8) >= srcdata.Length)
                        {
                            for (int k = 0; k < search_list[i].Length; k++)
                                srcdata[commLoc + k] = 0;
                            break;
                        }

                        for (int k = 0; k < recvdatalen + 8; k++)
                            recvsum += srcdata[commLoc + search_list[i].Length + 4 + 4 + k];
                        if (recvsum == recvdatacheck)
                        {
                            byte[] deviceid = new byte[8];
                            byte[] data = null;
                            Array.Copy(srcdata, commLoc + search_list[i].Length + 4 + 4, deviceid, 0, 8);
                            if (recvdatalen > 0)
                            {
                                data = new byte[recvdatalen];
                                Array.Copy(srcdata, commLoc + search_list[i].Length + 4 + 4 + 8, data, 0, recvdatalen);
                            }
                            TransData_Struct recv = new TransData_Struct
                            {
                                Keyword = (Protocol_Keyword)i,
                                Device_ID = deviceid,
                                Data = data
                            };
                            RecvArray.Add(recv);
                            for (int k = 0; k < search_list[i].Length; k++)
                                srcdata[commLoc + k] = 0;
                            deviceid = null;
                            data = null;
                            isSearch = true;
                            break;
                        }
                        else
                        {
                            if (recvfindlist[i].Count - 1 != j)
                            {
                                for (int k = 0; k < search_list[i].Length; k++)
                                    srcdata[commLoc + k] = 0;
                            }
                        }
                    }
                }
            }
        }



        public static void AnalysisRawData_2(ref byte[] srcdata,ref List<TransData_Struct> RecvArray)
        {
            bool isSearch = true;
            while (isSearch)
            {
                //string str = System.Text.Encoding.ASCII.GetString(srcdata);
                //str = str.Replace('\0', '*');
                isSearch = false;
                foreach (var item in Dic_Protocol)
                {
                    //int commLoc = str.IndexOf(item.Value.State_string);
                    bool isonly = true;
                    byte[] strdata = Encoding.Default.GetBytes(item.Value.State_string);
                    int commLoc = Search_ByteArray_String(ref srcdata,ref strdata, ref isonly);

                    if (commLoc >= 0)
                    {
                        if ((commLoc + item.Value.State_string.Length + 4 + 4) >= srcdata.Length)
                        {
                            for (int i = 0; i < strdata.Length; i++)
                                srcdata[commLoc + i] = 0;
                            break;
                        }



                        int recvdatalen = srcdata[commLoc + item.Value.State_string.Length] << 24;
                        recvdatalen |= srcdata[commLoc + item.Value.State_string.Length + 1] << 16;
                        recvdatalen |= srcdata[commLoc + item.Value.State_string.Length + 2] << 8;
                        recvdatalen |= srcdata[commLoc + item.Value.State_string.Length + 3];
                        int recvdatacheck = srcdata[commLoc + item.Value.State_string.Length + 4] << 24;
                        recvdatacheck |= srcdata[commLoc + item.Value.State_string.Length + 4 + 1] << 16;
                        recvdatacheck |= srcdata[commLoc + item.Value.State_string.Length + 4 + 2] << 8;
                        recvdatacheck |= srcdata[commLoc + item.Value.State_string.Length + 4 + 3];


                        int recvsum = 0;
                        for (int i = 0; i < strdata.Length; i++)
                            recvsum += strdata[i];
                        if (recvdatalen > 1024 * 20 || (commLoc + item.Value.State_string.Length + 4 + 4 + recvdatalen + 8) >= srcdata.Length)
                        {
                            for (int i = 0; i < strdata.Length; i++)
                                srcdata[commLoc + i] = 0;
                            break;
                        }
                           
                        for (int i = 0; i < recvdatalen + 8; i++)
                            recvsum += srcdata[commLoc + item.Value.State_string.Length + 4 + 4 + i];
                        if (recvsum == recvdatacheck)
                        {
                            byte[] deviceid = new byte[8];
                            byte[] data = null;
                            Array.Copy(srcdata, commLoc + item.Value.State_string.Length + 4 + 4, deviceid, 0, 8);
                            if (recvdatalen > 0)
                            {
                                data = new byte[recvdatalen];
                                Array.Copy(srcdata, commLoc + item.Value.State_string.Length + 4 + 4 + 8, data, 0, recvdatalen);
                            }
                            TransData_Struct recv = new TransData_Struct
                            {
                                Keyword = item.Key,
                                Device_ID = deviceid,
                                Data = data
                            };
                            RecvArray.Add(recv);
                            for (int i = 0; i < strdata.Length; i++)
                                srcdata[commLoc + i] = 0;
                            deviceid = null;
                            data = null;
                            isSearch = true;
                            break;
                        }
                        else
                        {
                            if (isonly == false)
                            {
                                for (int i = 0; i < strdata.Length; i++)
                                    srcdata[commLoc + i] = 0;
                            }
                        }
                        //for (int i = 0; i < strdata.Length; i++)
                        //    srcdata[commLoc + i] = 0;
                        strdata = null;
                        //if (recvsum == recvdatacheck)
                        //{
                        //    isSearch = true;
                        //    break;
                        //}
                    }
                }
            }
        }

        public static byte[] Construct_Data_Packet(TransData_Struct stu)
        {
            int sum = 0;
            byte[] data = new byte[Dic_Protocol[stu.Keyword].State_string.Length + stu.Data.Length + 8 + 8];
            byte[] strdata = Encoding.Default.GetBytes(Dic_Protocol[stu.Keyword].State_string);
            for (int i = 0; i < Dic_Protocol[stu.Keyword].State_string.Length; i++)
            {
                sum += strdata[i];
                data[i] = strdata[i];
            }
            for (int i = 0; i < stu.Device_ID.Length; i++)
            {
                sum += stu.Device_ID[i];
                data[i + Dic_Protocol[stu.Keyword].State_string.Length + 8] = stu.Device_ID[i];
            }
            for (int i = 0; i < stu.Data.Length; i++)
            {
                sum += stu.Data[i];
                data[i + 16 + Dic_Protocol[stu.Keyword].State_string.Length] = stu.Data[i];
            }
            data[Dic_Protocol[stu.Keyword].State_string.Length] = (byte)(stu.Data.Length >> 24);
            data[Dic_Protocol[stu.Keyword].State_string.Length + 1] = (byte)(stu.Data.Length >> 16);
            data[Dic_Protocol[stu.Keyword].State_string.Length + 2] = (byte)(stu.Data.Length >> 8);
            data[Dic_Protocol[stu.Keyword].State_string.Length + 3] = (byte)(stu.Data.Length);

            data[Dic_Protocol[stu.Keyword].State_string.Length + 4] = (byte)(sum >> 24);
            data[Dic_Protocol[stu.Keyword].State_string.Length + 5] = (byte)(sum >> 12);
            data[Dic_Protocol[stu.Keyword].State_string.Length + 6] = (byte)(sum >> 8);
            data[Dic_Protocol[stu.Keyword].State_string.Length + 7] = (byte)(sum);
            return data;
        }

        public static object State_Response_Function(TransData_Struct stu)
        {
            foreach (var item in Dic_Protocol)
            {
                if (stu.Keyword == item.Key)
                    return item.Value.Function_Event(stu);
            }
            return null;
        }
        
    }
}
