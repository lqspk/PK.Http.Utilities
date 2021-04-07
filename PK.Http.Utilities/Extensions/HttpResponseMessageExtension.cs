using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PK.Http.Utilities
{
    /// <summary>
    /// HttpResponseMessage扩展类
    /// </summary>
    public static class HttpResponseMessageExtension
    {
        /// <summary>
        /// 请求开始时间键
        /// </summary>
        internal const string BeginTimeHeaderKey = "__btime";

        /// <summary>
        /// 请求结束时间键
        /// </summary>
        internal const string EndTimeHeaderKey = "__etime";

        /// <summary>
        /// 创建异常的HttpResponseMessage
        /// </summary>
        /// <param name="e">异常</param>
        /// <param name="statusCode">范围从0-999</param>
        /// <returns></returns>
        internal static HttpResponseMessage CreateExceptionResponse(Exception e, int statusCode = 600)
        {
            return new HttpResponseMessage((HttpStatusCode)statusCode)
            {
                ReasonPhrase = GetExceptionMessages(e)
            };
        }

        /// <summary>
        /// 创建异常的HttpResponseMessage
        /// </summary>
        /// <param name="e">异常</param>
        /// <param name="requestMessage">请求消息类</param>
        /// <param name="statusCode">范围从0-999</param>
        /// <returns></returns>
        internal static HttpResponseMessage CreateExceptionResponse(Exception e, HttpRequestMessage requestMessage, int statusCode = 600)
        {
            return new HttpResponseMessage((HttpStatusCode)statusCode)
            {
                ReasonPhrase = GetExceptionMessages(e),
                RequestMessage =requestMessage
            };
        }

        /// <summary>
        /// 获取请求日志
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="getRequestBody">是否获取RequestBody</param>
        /// <param name="getResponseBody">是否获取ResponseBody</param>
        /// <returns></returns>
        public static async Task<HttpClientRequestLog> GetRequestLogAsync(this HttpResponseMessage responseMessage,
            bool getRequestBody = true, bool getResponseBody = true)
        {
            //请求开始时间
            responseMessage.Headers.TryGetValues(BeginTimeHeaderKey, out IEnumerable<string> requestBeginTime);
            DateTime.TryParse(requestBeginTime?.FirstOrDefault(), out DateTime beginTime);

            //请求结束时间
            responseMessage.Headers.TryGetValues(EndTimeHeaderKey, out IEnumerable<string> requestEndTime);
            DateTime.TryParse(requestEndTime?.FirstOrDefault(), out DateTime endTime);

            //创建日志类
            var log = new HttpClientRequestLog()
            {
                StartTime = beginTime,
                EndTime = endTime,
                RequestUrl =
                    $"{responseMessage.RequestMessage?.RequestUri.Scheme}://{responseMessage.RequestMessage?.RequestUri.Authority}",
                RequestHeaders = responseMessage.RequestMessage?.Headers
                    .Select(s => new KeyValuePair<string, string>(s.Key, s.Value.ToString())).ToArray(),
                Method = responseMessage.RequestMessage?.Method.Method,
                RequestQuery = responseMessage.RequestMessage?.RequestUri.Query,
                ResponseHeaders = responseMessage.Headers
                    .Select(s => new KeyValuePair<string, string>(s.Key, s.Value.ToString())).ToArray(),
                ResponseHttpStatusCode = (int) responseMessage.StatusCode,
                ResponseReasonPhrase = responseMessage.ReasonPhrase
            };

            try
            {
                if (getRequestBody)
                    log.RequestBody = responseMessage.RequestMessage?.Content != null
                        ? await responseMessage.RequestMessage.Content.ReadAsStringAsync()
                        : null;
            }
            catch (Exception e)
            {
                log.RequestBody = GetExceptionMessages(e);
            }

            try
            {
                if (getResponseBody)
                    log.ResponseBody = responseMessage.Content != null
                        ? await responseMessage.Content.ReadAsStringAsync()
                        : null;
            }
            catch (Exception e)
            {
                log.ResponseBody = GetExceptionMessages(e);
            }

            return log;
        }

        /// <summary>
        /// 递归获取异常消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static string GetExceptionMessages(Exception e)
        {
            List<string> messageList = new List<string>();
            var exception = e;
            while (exception != null)
            {
                if (!string.IsNullOrWhiteSpace(exception.Message))
                {
                    messageList.Add(exception.Message.TrimEnd('。', '！', '；'));
                }
                exception = exception.InnerException;
            }

            return string.Join("；", messageList);
        }


    }

}
