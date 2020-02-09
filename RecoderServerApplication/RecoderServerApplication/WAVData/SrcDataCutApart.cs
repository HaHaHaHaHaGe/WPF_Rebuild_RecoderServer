using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoderServerApplication.WAVData
{
    class SrcDataCutApart
    {
        private List<Speex_Creater> List_WavFile = new List<Speex_Creater>();
        private int Frequency;
        private uint singlefile_size;
        private string dirstr;
        private int single_time;

        public class Error_Statistical
        {
            public string file;
            public uint Repair;
            public List<Speex_Creater.DeformityData> errorlist;
        }

        public SrcDataCutApart(int fre,int single_file_time, string dir)
        {
            Frequency = fre;
            single_time = single_file_time;
            singlefile_size = (uint)(single_file_time * 2 * fre / 16);
            dirstr = dir;
        }
        public void WriteWavData(byte[] wavdata , uint start_index , DateTime create_first_time)
        {
            uint file_num = start_index / singlefile_size;
            uint file_buff_num = (uint)(wavdata.Length + start_index - 1) / singlefile_size;

            uint file_relv_loc = start_index % singlefile_size;
            uint surplusdata_len = (uint)wavdata.Length;


            while (file_buff_num >= List_WavFile.Count)
            {
                if(List_WavFile.Count > 0)
                    List_WavFile[List_WavFile.Count - 1].Full_File_NotClose();
                List_WavFile.Add(new Speex_Creater(dirstr + create_first_time.AddSeconds(List_WavFile.Count * single_time).ToString("yyyy-MM-dd-HH-mm-ss") +"-000_REC"+ ".wzr", Frequency, singlefile_size));
            }

            if (surplusdata_len > singlefile_size - file_relv_loc)
            {
                List_WavFile[(int)file_num].WirteWavFile(wavdata, file_relv_loc, singlefile_size - file_relv_loc);
                surplusdata_len -= singlefile_size - file_relv_loc;
                file_num++;
                Array.Copy(wavdata, singlefile_size - file_relv_loc,wavdata,0,wavdata.Length - (singlefile_size - file_relv_loc));
                file_relv_loc = 0;
            }
            
            while (surplusdata_len != 0)
            {
                if (surplusdata_len >= singlefile_size)
                {
                    List_WavFile[(int)file_num].WirteWavFile(wavdata, 0, singlefile_size);
                    Array.Copy(wavdata, singlefile_size, wavdata, 0, wavdata.Length - singlefile_size);
                    surplusdata_len -= singlefile_size;
                }
                else
                {
                    List_WavFile[(int)file_num].WirteWavFile(wavdata, file_relv_loc, surplusdata_len);
                    surplusdata_len = 0;
                }
                file_num++;
            }
        }

        public void CloseAllWavFile()
        {
            if (List_WavFile.Count == 0)
                return;
            for(int i = 0;i < List_WavFile.Count-1;i++)
            {
                List_WavFile[i].CloseWavFile(true);
            }
            List_WavFile[List_WavFile.Count - 1].CloseWavFile(false);
            List_WavFile.Clear();
        }

        public int GetAllWav_ERROR_Number()
        {
            int sum = 0;
            for (int i = 0; i < List_WavFile.Count; i++)
            {
                sum += List_WavFile[i].Get_ERROR_Number();
            }
            return sum;
        }

        public int GetAllWav_Repair_Number()
        {
            int sum = 0;
            for (int i = 0; i < List_WavFile.Count; i++)
            {
                sum += (int)List_WavFile[i].repair_times;
            }
            return sum;
        }

        public List<Error_Statistical> DetailedErrorData()
        {
            List<Error_Statistical> recv = new List<Error_Statistical>();
            for (int i = 0; i < List_WavFile.Count; i++)
            {
                Error_Statistical error = new Error_Statistical();
                error.file = List_WavFile[i].filedir;
                error.errorlist = new List<Speex_Creater.DeformityData>();
                List<Speex_Creater.DeformityData> buf = List_WavFile[i].Get_Deformity_Data();

                for(int j = 0; j < buf.Count; j++)
                {
                    error.errorlist.Add(new Speex_Creater.DeformityData(buf[j].start_location, buf[j].end_location));
                }


                for (int j = 0; j < error.errorlist.Count; j++) 
                {
                    error.errorlist[j].start_location += (uint)(i * singlefile_size);
                    error.errorlist[j].end_location += (uint)(i * singlefile_size);
                }

                error.Repair = List_WavFile[i].repair_times;
                recv.Add(error);
            }
            return recv;
        }
    }
}
