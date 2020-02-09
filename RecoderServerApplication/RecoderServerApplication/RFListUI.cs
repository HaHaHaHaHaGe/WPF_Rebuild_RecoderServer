using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoderServerApplication
{
    class RFListUI
    {
        public static List<UI_Trans> listdata = new List<UI_Trans>();
        public class UI_Trans
        {
            public int Index { get; set; }
            public string ID { get; set; }
            public string Bind { get; set; }
        }

        
    }
    
}
