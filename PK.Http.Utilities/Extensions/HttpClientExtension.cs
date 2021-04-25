using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PK.Http.Utilities
{
    /// <summary>
    /// HttpClient扩展类
    /// </summary>
    public sealed class HttpClientExtension : IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly MessageProcessingHandlerExtension _messageProcessingHandler;


        #region 公开属性

        public HttpRequestHeaders DefaultRequestHeaders => this._httpClient.DefaultRequestHeaders;

        public Uri BaseAddress => this._httpClient.BaseAddress;

        public long MaxResponseContentBufferSize => this._httpClient.MaxResponseContentBufferSize;

        public TimeSpan Timeout => this._httpClient.Timeout;

        public HttpClientHandlerExtension HttpClientHandler => (HttpClientHandlerExtension)this._messageProcessingHandler.InnerHandler;

        #endregion

        internal HttpClientExtension(HttpClient httpClient)
        {
            this._httpClient = httpClient;
            this._messageProcessingHandler = GetMessageProcessingHandler(this._httpClient);
        }

        public void Dispose()
        {
            this._httpClient?.Dispose();
        }

        public void CancelPendingRequests()
        {
            this._httpClient.CancelPendingRequests();
        }

        /// <summary>
        /// 异步Get方法
        /// </summary>
        /// <param name="requestUrl">请求链接</param>
        /// <param name="cookie">请求Cookie(key=value格式)</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string requestUrl, string cookie = null)
        {
            DateTime requestStartTime = DateTime.Now; //请求开始时间
            DateTime requestEndTime = requestStartTime; //请求结束时间

            //创建请求对象
            Uri uri = new Uri(requestUrl);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUrl));

            //返回对象
            HttpResponseMessage responseMessage = null;

            try
            {
                //由于HttpClientHandler.UseCookies默认值是true，所以要设置为false。
                this.HttpClientHandler.UseCookies = false;

                //判断是否有Cookie
                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    requestMessage.Headers.Add("Cookie", cookie);
                }

                //如果是https请求
                if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
                                                           SecurityProtocolType.Tls;
                }

                requestStartTime = DateTime.Now;
                responseMessage = await this._httpClient.SendAsync(requestMessage);
                requestEndTime = DateTime.Now;
            }
            catch (HttpRequestException e)
            {
                requestEndTime = DateTime.Now;
                responseMessage = HttpResponseMessageExtension.CreateExceptionResponse(e, requestMessage);
            }
            finally
            {
                responseMessage.Headers.Add(HttpResponseMessageExtension.BeginTimeHeaderKey,
                    requestStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                responseMessage.Headers.Add(HttpResponseMessageExtension.EndTimeHeaderKey,
                    requestEndTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                //因为在发送请求后有可能会被改变，所以要重新赋值
                responseMessage.RequestMessage.RequestUri = uri;
            }

            return responseMessage;
        }

        /// <summary>
        /// 异步Post方法
        /// </summary>
        /// <param name="requestUrl">请求链接</param>
        /// <param name="postBody">请求Body内容</param>
        /// <param name="cookie">请求Cookie(key=value格式)</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string requestUrl, HttpContent postBody = null, string cookie = null)
        {
            DateTime requestStartTime = DateTime.Now; //请求开始时间
            DateTime requestEndTime = requestStartTime; //请求结束时间

            //创建请求对象
            Uri uri = new Uri(requestUrl);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            //返回对象
            HttpResponseMessage responseMessage = null;
            
            try
            {
                //由于HttpClientHandler.UseCookies默认值是true，所以要设置为false。
                this.HttpClientHandler.UseCookies = false;

                //判断是否有Cookie
                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    requestMessage.Headers.Add("Cookie", cookie);
                }

                //判断是否有请求体
                if (postBody != null)
                {
                    //复制Body，在发送请求后会被释放
                    using (Stream stream = new MemoryStream())
                    {
                        await postBody.CopyToAsync(stream);
                        stream.Position = 0;
                        HttpContent postBodyCopy = new StreamContent(stream);

                        //复制请求头，排除已存在的Key
                        foreach (var bodyHeader in postBody.Headers.Where(s =>
                            postBodyCopy.Headers.Select(q => q.Key).Contains(s.Key) == false).ToList())
                        {
                            postBodyCopy.Headers.Add(bodyHeader.Key, bodyHeader.Value);
                        }

                        //赋值到请求对象
                        requestMessage.Content = postBodyCopy;
                    }
                }

                //如果是https请求
                if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
                                                           SecurityProtocolType.Tls;
                }

                requestStartTime = DateTime.Now;
                responseMessage = await this._httpClient.SendAsync(requestMessage);
                requestEndTime = DateTime.Now;
            }
            catch (HttpRequestException e)
            {
                requestEndTime = DateTime.Now;
                responseMessage = HttpResponseMessageExtension.CreateExceptionResponse(e, requestMessage);
            }
            finally
            {
                responseMessage.Headers.Add(HttpResponseMessageExtension.BeginTimeHeaderKey,
                    requestStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                responseMessage.Headers.Add(HttpResponseMessageExtension.EndTimeHeaderKey,
                    requestEndTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                //因为postBody在发送请求后会被释放，所以要重新赋值
                responseMessage.RequestMessage.Content = postBody;

                //因为在发送请求后有可能会被改变，所以要重新赋值
                responseMessage.RequestMessage.RequestUri = uri;
            }

            return responseMessage;
        }

        #region 私有方法

        /// <summary>
        /// 从HttpClient获取MessageProcessingHandler
        /// </summary>
        /// <param name="instance">HttpClient实例</param>
        /// <returns></returns>
        private MessageProcessingHandlerExtension GetMessageProcessingHandler(object instance)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType().BaseType;
            FieldInfo field = null;
            if (type.Name == nameof(HttpMessageInvoker))
            {
                field = type.GetField("handler", flag);
            }
            else
            {
                field = type.GetField("innerHandler", flag);
            }

            var result = field.GetValue(instance);
            while (result.GetType().Name != nameof(MessageProcessingHandlerExtension))
            {
                result = GetMessageProcessingHandler(result);
            }

            return (MessageProcessingHandlerExtension)result;
        }

        #endregion
    }
}
