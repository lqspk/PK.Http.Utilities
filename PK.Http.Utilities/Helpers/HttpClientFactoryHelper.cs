using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace PK.Http.Utilities
{
    /// <summary>
    /// HttpClientFactory帮助类
    /// </summary>
    public class HttpClientFactoryHelper
    {
        private static readonly IHttpClientFactory _httpClientFactory;
        static HttpClientFactoryHelper()
        {
            var serviceProvider = new ServiceCollection()
                .AddHttpClient("HttpClientExtension")
                .ConfigurePrimaryHttpMessageHandler(() => new MessageProcessingHandlerExtension
                {
                    InnerHandler = new HttpClientHandlerExtension()
                })
                .Services
                .BuildServiceProvider();

            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        }

        /// <summary>
        /// 创建一个HttpClientExtension
        /// </summary>
        /// <returns></returns>
        public static HttpClientExtension CreateClient()
        {
            var client = _httpClientFactory.CreateClient("HttpClientExtension");

            return new HttpClientExtension(client);
        }

    }
}