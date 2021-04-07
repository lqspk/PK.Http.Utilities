using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PK.Http.Utilities
{
    /// <summary>
    /// HttpClientHandler扩展类
    /// </summary>
    public sealed class HttpClientHandlerExtension : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
