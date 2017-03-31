using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Helpers
{
    public interface IHttpClientHelper
    {
        Task<string> GetAsync(Uri uri, Dictionary<string, string> headers, string bearerToken, CancellationToken cancellationToken);
        Task<byte[]> GetBytesAsync(Uri uri, Dictionary<string, string> headers, string bearerToken, CancellationToken cancellationToken);
        Task<string> PostAsync(Uri uri, string requestJson, string bearerToken, CancellationToken cancellationToken);
    }
}