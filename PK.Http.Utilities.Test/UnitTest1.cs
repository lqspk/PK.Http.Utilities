using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace PK.Http.Utilities.Test
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// 测试Get请求
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetAsync()
        {
            var httpClient = HttpClientFactoryHelper.CreateClient();

            var resMessage = await httpClient.GetAsync("http://localhost:63447/Home/TestGet?name=test");
            var log = await resMessage.GetRequestLogAsync();
            if (resMessage.IsSuccessStatusCode)
            {
                var content = await resMessage.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine(resMessage.ReasonPhrase);
            }
        }

        /// <summary>
        /// 测试Post请求
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestPostAsync()
        {
            var httpClient = HttpClientFactoryHelper.CreateClient();

            var resMessage = await httpClient.PostAsync("http://localhost:63447/Home/TestPost?name=test");
            var log = await resMessage.GetRequestLogAsync();
            if (resMessage.IsSuccessStatusCode)
            {
                var content = await resMessage.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine(resMessage.ReasonPhrase);
            }
        }

        /// <summary>
        /// 测试json请求
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestPostStringContentAsync()
        {
            var httpClient = HttpClientFactoryHelper.CreateClient();

            using (var postBody = new StringContent(JsonConvert.SerializeObject(new {@name = "test"}), Encoding.UTF8,
                "application/json"))
            {
                var resMessage = await httpClient.PostAsync("http://localhost:63447/api/value/TestPost", postBody);
                var log = await resMessage.GetRequestLogAsync();
                if (resMessage.IsSuccessStatusCode)
                {
                    var content = await resMessage.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine(resMessage.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// 测试Post FormUrlEncodedContent请求
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestPostFormUrlEncodedContentAsync()
        {
            var httpClient = HttpClientFactoryHelper.CreateClient();

            using (var postBody = new FormUrlEncodedContent(new KeyValuePair<string, string>[]{new KeyValuePair<string, string>("name", "test")}))
            {
                var resMessage = await httpClient.PostAsync("http://localhost:63447/Home/TestPost", postBody);
                var log = await resMessage.GetRequestLogAsync();
                if (resMessage.IsSuccessStatusCode)
                {
                    var content = await resMessage.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine(resMessage.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// 测试Post MultipartFormDataContent
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestPostMultipartFormDataContentAsync()
        {
            var httpClient = HttpClientFactoryHelper.CreateClient();

            using (var postBody = new MultipartFormDataContent())
            {
                string boundary = string.Format("--{0}", DateTime.Now.Ticks.ToString("x"));
                postBody.Headers.Add("ContentType", $"multipart/form-data, boundary={boundary}");

                //data为请求文件接口需要的参数，根据调用接口参数而定
                StringContent data = new StringContent(JsonConvert.SerializeObject(new {@name = "test"}));
                postBody.Add(data, "data");

                StringContent data1 = new StringContent(JsonConvert.SerializeObject(new { @name1 = "test1" }));
                postBody.Add(data1, "data1");

                var resMessage = await httpClient.PostAsync("http://localhost:63447/Home/TestMultipartContent", postBody);
                var log = await resMessage.GetRequestLogAsync();
                if (resMessage.IsSuccessStatusCode)
                {
                    var content = await resMessage.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine(resMessage.ReasonPhrase);
                }
            }
        }
    }
}

