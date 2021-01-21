using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoderServerApplication.Tools
{
    class DataRecovery
    {
        public static string getLastAudioFolder()
        {
            DateTime newDirIndex = new DateTime(0);
            DirectoryInfo root = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\AudioFile");
            DirectoryInfo newDir = root;
            DirectoryInfo[] dics = root.GetDirectories();
            foreach (DirectoryInfo dic in dics)
            {
                DateTime time;
                try
                {
                    time = DateTime.ParseExact(dic.Name.Split('_')[0], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    
                }
                catch(Exception e)
                {
                    continue;
                }
                if(time > newDirIndex)
                {
                    newDirIndex = time;
                    newDir = dic;
                }
            }
            return newDir.FullName;
        }

        public static int getLastRecoderProgress(string dirName,string deviceID)
        {
            int recv = 0;
            string[] files = Directory.GetFiles(dirName, "*.wzr");
            DateTime newDirIndex = new DateTime(0);
            DateTime time;
            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);
                string[] sp = filename.Split('_');
                if(sp[1] == deviceID)
                {
                    time = DateTime.ParseExact(sp[2], "yyyy-MM-dd-HH-mm-ss-000", System.Globalization.CultureInfo.CurrentCulture);
                    if(time > newDirIndex)
                    {
                        newDirIndex = time;
                    }

                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Length > 0)
                    {
                        recv++;
                    }
                }
            }
            return recv;
        }
    }
}
