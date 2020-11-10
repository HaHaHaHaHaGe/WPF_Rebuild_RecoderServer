using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoderServerApplication.HttpPost
{
    class json
    {
        public class LoginUsers
        {
            public int index { get; set; }
            public string user_id { get; set; }
            public string nickname { get; set; }
        }

        public class LoginJsonResultStruct
        {
            public bool flag;
            public string activity_id;
            public string user_token;
            public string message;
            public List<LoginUsers> users = new List<LoginUsers>();
        }
        public static LoginJsonResultStruct recv = new LoginJsonResultStruct();
        public static LoginJsonResultStruct LoginJsonResult(string JsonText)
        {
            recv.users.Clear();
            recv.flag = false;
            string[] str = new string[7];
            var json2 = (JObject)JsonConvert.DeserializeObject(JsonText);
            recv.flag = json2["flag"].ToString() == "1" ? true : false;
            recv.message = json2["message"].ToString();


            //var json3 = (JObject)JsonConvert.DeserializeObject(Object.ToString());
            if (recv.flag)
            {
                recv.activity_id = json2["activity_id"].ToString();
                recv.user_token = json2["user_token"].ToString();
                var Object = json2["users"];
                var arrdata = Newtonsoft.Json.Linq.JArray.Parse(Object.ToString());
                recv.users = arrdata.ToObject<List<LoginUsers>>();
            }
            return recv;
        }
        public static LoginJsonResultStruct RefreshJsonResult(string JsonText)
        {
            recv.users.Clear();
            recv.flag = false;
            string[] str = new string[7];
            var json2 = (JObject)JsonConvert.DeserializeObject(JsonText);
            recv.flag = json2["flag"].ToString() == "1" ? true : false;
            recv.message = json2["message"].ToString();


            //var json3 = (JObject)JsonConvert.DeserializeObject(Object.ToString());
            if (recv.flag)
            {
               // recv.activity_id = json2["activity_id"].ToString();
               // recv.user_token = json2["user_token"].ToString();
                var Object = json2["users"];
                var arrdata = Newtonsoft.Json.Linq.JArray.Parse(Object.ToString());
                recv.users = arrdata.ToObject<List<LoginUsers>>();
            }
            return recv;
        }

        public static LoginJsonResultStruct InsertJsonResult(string JsonText)
        {
            recv.users.Clear();
            recv.flag = false;
            string[] str = new string[7];
            var json2 = (JObject)JsonConvert.DeserializeObject(JsonText);
            recv.flag = json2["flag"].ToString() == "1" ? true : false;
            recv.message = json2["message"].ToString();


            //var json3 = (JObject)JsonConvert.DeserializeObject(Object.ToString());
            if (recv.flag)
            {
                // recv.activity_id = json2["activity_id"].ToString();
                // recv.user_token = json2["user_token"].ToString();
                //recv.message = json2["message"].ToString();
            }
            return recv;
        }
        public static LoginJsonResultStruct LogoutJsonResult(string JsonText)
        {
            try
            {
                recv.users.Clear();
                recv.flag = false;
                string[] str = new string[7];
                var json2 = (JObject)JsonConvert.DeserializeObject(JsonText);
                recv.flag = json2["flag"].ToString() == "1" ? true : false;
                recv.message = json2["message"].ToString();

            }
            catch(Exception e)
            {

            }
            //var json3 = (JObject)JsonConvert.DeserializeObject(Object.ToString());
            if (recv.flag)
            {
                // recv.activity_id = json2["activity_id"].ToString();
                // recv.user_token = json2["user_token"].ToString();
                //recv.message = json2["message"].ToString();
            }
            return recv;
        }
    }
}
