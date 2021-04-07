using System;
using System.Collections.Generic;
using System.Text;

namespace PK.Http.Utilities
{
    /// <summary>
    /// 请求日志数据模型
    /// </summary>
    public class HttpClientRequestLog
    {
        /// <summary>
        /// 请求开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 请求结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 请求的url
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求方法GET或者是POST
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        public KeyValuePair<string, string>[] RequestHeaders { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestQuery { get; set; }

        /// <summary>
        /// 请求体
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// 返回头
        /// </summary>
        public KeyValuePair<string, string>[] ResponseHeaders { get; set; }

        /// <summary>
        /// 返回体
        /// </summary>
        public string ResponseBody { get; set; }

        /// <summary>
        /// 返回状态码
        /// </summary>
        public int ResponseHttpStatusCode { get; set; }

        /// <summary>
        /// 返回状态码中文
        /// </summary>
        public string ResponseReasonPhrase { get; set; }

        /// <summary>
        /// 请求耗费时间（毫秒）
        /// </summary>
        public long RequestTimeConsuming => (long)(EndTime - StartTime).TotalMilliseconds;
    }
}
