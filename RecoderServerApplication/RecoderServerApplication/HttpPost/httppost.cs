using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace RecoderServerApplication.SQLite
{
    class httppost
    {
        public static string HttpPost(string Url, string postDataStr, ref bool isSuccess)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                //request.CookieContainer = cookie;
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }

            catch (Exception e)
            {
                isSuccess = false;
                Console.Write(e.Message);
                return e.Message;
            }
        }
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求 
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout;

            //if (cookies != null)
            //{
            //    request.CookieContainer = new CookieContainer();
            //    request.CookieContainer.Add(cookies);
            //}
            //发送POST数据 
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();

            }
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }


        public static bool uploadfile(string filepath,string filename)
        {
            HttpPost.json.LoginJsonResultStruct data2;
            try
            {
                WebClient webClient = new WebClient();  // 初始化WebClient
                byte[] data = webClient.UploadFile("https://classroom.talkingbrain.com.cn/index.php/home/api/post_file?user_token=" + RecoderServerApplication.HttpPost.json.recv.user_token + "&file=" + filename, "POST", filepath+ filename);  // 上传文件
                string recv = Encoding.UTF8.GetString(data);
                data2 = RecoderServerApplication.HttpPost.json.LogoutJsonResult(recv);
                webClient.Dispose();
            }
            catch(Exception e)
            {
                Thread.Sleep(2000);
                return false;

            }
            
            return data2.flag;
        }
    }
}
