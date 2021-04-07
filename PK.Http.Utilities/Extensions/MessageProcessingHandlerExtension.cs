using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace PK.Http.Utilities
{
    /// <summary>
    /// MessageProcessingHandler扩展类
    /// </summary>
    internal class MessageProcessingHandlerExtension : MessageProcessingHandler
    {
        /// <summary>
        /// 处理请求方法
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return request;
        }

        /// <summary>
        /// 处理返回方法
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            return response;
        }
    }
}
