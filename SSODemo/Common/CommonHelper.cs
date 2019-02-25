using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CommonHelper
    {
        public static ResultEx HttpClientPost(string url, string data)
        {
            //设置Http的正文
            HttpContent httpContent = new StringContent(data);
            //设置Http的内容标头
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //设置Http的内容标头的字符
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient client = new HttpClient())
            {
                //异步Post
                HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
                //确保Http响应成功
                if (response.IsSuccessStatusCode)
                {
                    //异步读取json
                    string str = response.Content.ReadAsStringAsync().Result;
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ResultEx>(str);
                }
                return new ResultEx { Flag = false };

            }
        }
    }
}
