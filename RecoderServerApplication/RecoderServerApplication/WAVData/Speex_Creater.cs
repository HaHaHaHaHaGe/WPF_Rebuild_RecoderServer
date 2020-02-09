using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RecoderServerApplication.WAVData
{
    class Speex_Creater
    {
        private __WaveHeader wavhead;
        private FileStream file;
        private BinaryWriter bin;
        private uint wavdatalen = 0;
        private uint maxwavdatalen = 0;
        public uint repair_times = 0;
        private byte[] wavdata;
        private List<DeformityData> Deformity_Data = new List<DeformityData>();
        public string filedir;
        struct ChunkDATA
        {
            public UInt32 ChunkID;
            public UInt32 ChunkSize;
        };

        struct ChunkRIFF
        {
            public UInt32 ChunkID;         
            public UInt32 ChunkSize;           
            public UInt32 Format;              
        };

        struct ChunkFMT
        {
            public UInt32 ChunkID;           
            public UInt32 ChunkSize;         
            public UInt16 AudioFormat;        
            public UInt16 NumOfChannels;      
            public UInt32 SampleRate;        
            public UInt32 ByteRate;           
            public UInt16 BlockAlign;          
            public UInt16 BitsPerSample;       
        };

        struct __WaveHeader
        {
            public ChunkRIFF riff; 
            public ChunkFMT fmt;    
            public ChunkDATA data;  	 
        };

        public class DeformityData
        {
            public DeformityData(uint start,uint end)
            {
                start_location = start;
                end_location = end;
            }
            public uint start_location;
            public uint end_location;
        }

        
        private static byte[] StructToBytes(object structObj, int size)
        {
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;

        }

        public Speex_Creater(string dirstr, int fs, uint wavlen)
        {
            filedir = dirstr;
            CreateWavFile(dirstr,fs,wavlen);
        }

        public void CreateWavFile(string dirstr,int fs,uint wavlen)
        {
            wavhead.riff.ChunkID = 0X46464952; 
            wavhead.riff.ChunkSize = 0;           
            wavhead.riff.Format = 0X45564157;  
            wavhead.fmt.ChunkID = 0X20746D66;  
            wavhead.fmt.ChunkSize = 16;
            wavhead.fmt.AudioFormat = 0X01;        
            wavhead.fmt.NumOfChannels = 1;   
            wavhead.fmt.SampleRate = (uint)fs;      
            wavhead.fmt.ByteRate = wavhead.fmt.SampleRate * 2;
            wavhead.fmt.BlockAlign = 2;           
            wavhead.fmt.BitsPerSample = 16;       
            wavhead.data.ChunkID = 0X61746164; 
            wavhead.data.ChunkSize = 0;        
                                               
            file = new FileStream(dirstr, FileMode.Create);
            bin = new BinaryWriter(file);
            wavdata = new byte[wavlen];
            maxwavdatalen = wavlen;
        }

        public void WirteWavFile(byte[] data,uint index, uint dataLength)
        {
            if (file == null)
                return;


            if(index > wavdatalen)
            {
                Deformity_Data.Add(new DeformityData(wavdatalen, index));
            }
            if (index < wavdatalen)
            {
                for (int i = 0; i < Deformity_Data.Count; i++)
                {
                    if (index == Deformity_Data[i].start_location && (dataLength + index) == Deformity_Data[i].end_location)
                    {
                        repair_times++;
                        Deformity_Data.RemoveAt(i);
                        break;
                    }


                    if (index <= Deformity_Data[i].start_location && (dataLength + index) < Deformity_Data[i].end_location && (dataLength + index) > Deformity_Data[i].start_location)
                    {
                        Deformity_Data[i].start_location = (dataLength + index);
                        repair_times++;
                    }


                    if (index > Deformity_Data[i].start_location && (dataLength + index) >= Deformity_Data[i].end_location && index < Deformity_Data[i].end_location)
                    {
                        Deformity_Data[i].end_location = index;
                        repair_times++;
                    }


                    if (index <= Deformity_Data[i].start_location && (dataLength + index) >= Deformity_Data[i].end_location)
                    {
                        Deformity_Data.RemoveAt(i);
                        i--;
                        repair_times++;
                        continue;
                    }


                    if (index > Deformity_Data[i].start_location && (dataLength + index) < Deformity_Data[i].end_location)
                    {
                        uint end_ = Deformity_Data[i].end_location;
                        Deformity_Data[i].end_location = index;
                        Deformity_Data.Add(new DeformityData((dataLength + index), end_));
                        repair_times++;
                        break;
                    }
                        

                }
            }
            Buffer.BlockCopy(data, 0, wavdata, (int)index, (int)dataLength);
            if(index + dataLength >= wavdatalen)
                wavdatalen = index + dataLength;

            if (wavdatalen == maxwavdatalen && Deformity_Data.Count == 0)
                CloseWavFile(true);
        }
        public void Full_File_NotClose()
        {
            if (file == null)
                return;
            if(wavdatalen < maxwavdatalen)
                Deformity_Data.Add(new DeformityData(wavdatalen, maxwavdatalen));
        }

        public void CloseWavFile(bool isfilldata)
        {
            if (file == null)
                return;
            if (isfilldata)
            {
                wavhead.riff.ChunkSize = maxwavdatalen + 36;
                wavhead.data.ChunkSize = maxwavdatalen;
            }
            else
            {
                wavhead.riff.ChunkSize = wavdatalen + 36;
                wavhead.data.ChunkSize = wavdatalen;
            }
            //byte[] b = StructToBytes(wavhead, 44);
            //bin.Write(b, 0, 44);
            bin.Write(wavdata, 0, (int)wavdatalen);
            bin.Close();
            file.Close();
            Process.Start("speex_decoder.exe" ,filedir);
            wavdata = null;
            Deformity_Data = null;
            bin = null;
            file = null;
        }

        public int Get_ERROR_Number()
        {
            if (Deformity_Data == null)
                return 0;
            return Deformity_Data.Count;
        }

        public List<DeformityData> Get_Deformity_Data()
        {
            if (Deformity_Data == null)
                return new List<DeformityData>();
            return Deformity_Data;
        }

    }
}
