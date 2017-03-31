using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Helpers
{
    public class HttpClientHelper : IHttpClientHelper
    {
        public async Task<string> PostAsync(Uri uri, string requestJson, string bearerToken, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(bearerToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                }

                //todo: Add retry logic here (Polly)

                var response = await client.PostAsync(uri, new StringContent(requestJson, Encoding.UTF8, "application/json"), cancellationToken);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<byte[]> GetBytesAsync(Uri uri, Dictionary<string, string> headers, string bearerToken, CancellationToken cancellationToken)
        {
            var response = await GetInternalAsync(uri, headers, bearerToken, cancellationToken);
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<string> GetAsync(Uri uri, Dictionary<string, string> headers, string bearerToken, CancellationToken cancellationToken)
        {
            var response = await GetInternalAsync(uri, headers, bearerToken, cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<HttpResponseMessage> GetInternalAsync(Uri uri, Dictionary<string, string> headers, string bearerToken, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(bearerToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                }

                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                //todo: Add retry logic here (Polly)

                var response = await client.GetAsync(uri, cancellationToken);

                response.EnsureSuccessStatusCode();

                return response;
            }
        }
    }
}