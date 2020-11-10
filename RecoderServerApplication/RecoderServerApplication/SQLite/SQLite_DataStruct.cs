using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoderServerApplication.SQLite
{
    public class SQLite_DataStruct
    {
        public int Index { get; set; }
        public string DeviceID { get; set; }
        public string UserID { get; set; }
        public string NickName { get; set; }
    }

    public class SQLite_InitData
    {
        public string init_Group { get; set; }
        public string init_WIFI { get; set; }
        public string init_PASS { get; set; }
        public string init_IP { get; set; }
        public string init_port { get; set; }
        public string init_Server_IP { get; set; }
        public string init_Server_Port { get; set; }
    }
}
