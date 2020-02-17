using RecoderServerApplication.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecoderServerApplication.MultiThread
{
    class UploadThread
    {
        public class UploadFiles
        {
            public string filepath { get; set; }
            public string filename { get; set; }
        }
        Thread Upload_Thread;
        static Queue<UploadFiles> _tasks = new Queue<UploadFiles>();
        readonly static object _locker = new object();
        static EventWaitHandle _wh = new AutoResetEvent(false);
        public static bool EnqueuelTask(UploadFiles data)
        {
            lock (_locker)
            {
                _tasks.Enqueue(data);
            }
            return true;
        }
        public UploadThread()
        {
            Upload_Thread = new Thread(Update_Function);
            Upload_Thread.Start();
            Upload_Thread.IsBackground = true;
        }
        private void Update_Function()
        {
            UploadFiles title = new UploadFiles();
            while (true)
            {
                lock (_locker) //锁，用来保护数据读写不会冲突
                {
                    if (_tasks.Count > 0) //队列中剩余的数据
                    {
                        title = _tasks.Dequeue(); //去取得数据
                    }
                }
                if (title.filename != null) //任务不为空
                {
                    bool recv =  httppost.uploadfile(title.filepath, title.filename);
                    if (recv == false)
                        EnqueuelTask(title);
                }
                else
                {
                    _wh.WaitOne();
                }
            }
        }
    }
}
