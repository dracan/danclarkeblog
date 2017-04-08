using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Dropbox;
using Newtonsoft.Json;

namespace DanClarkeBlog.Core.Helpers
{
    // See following link for reference (and live testing) of all the API endpoints
    // https://dropbox.github.io/dropbox-api-v2-explorer/

    public class DropboxHelper : IDropboxHelper
    {
        private readonly Settings _settings;
        private readonly IHttpClientHelper _httpClientHelper;

        private static readonly string DropboxApiUri = "https://api.dropboxapi.com";
        private static readonly string DropboxContentUri = "https://content.dropboxapi.com";

        public DropboxHelper(Settings settings, IHttpClientHelper httpClientHelper)
        {
            _settings = settings;
            _httpClientHelper = httpClientHelper;
        }

        public Task<List<string>> GetFilesAsync(string path, CancellationToken cancellationToken)
        {
            return GetFilesAsync(path, null, cancellationToken);
        }

        public async Task<List<string>> GetFilesAsync(string path, string cursor, CancellationToken cancellationToken)
        {
            var uri = new Uri(DropboxApiUri + "/2/files/list_folder");
            var continueUri = new Uri(DropboxApiUri + "/2/files/list_folder/continue");

            List<string> results;
            DropboxApiResponseListFiles response;

            if (cursor == null)
            {
                var jsonResponse = await _httpClientHelper.PostAsync(uri, $@"{{""path"":""{path}""}}", _settings.DropboxAccessToken, cancellationToken);

                response = JsonConvert.DeserializeObject<DropboxApiResponseListFiles>(jsonResponse);

                results = response.entries.Select(x => x.name).ToList();
            }
            else
            {
                results = new List<string>();
                response = new DropboxApiResponseListFiles {cursor = cursor, has_more = true};
            }

            while (response.has_more)
            {
                var json = $@"{{""path"":"""", ""cursor"": ""{response.cursor}""}}";
                var jsonResponse = await _httpClientHelper.PostAsync(continueUri, json, _settings.DropboxAccessToken, cancellationToken);
                response = JsonConvert.DeserializeObject<DropboxApiResponseListFiles>(jsonResponse);
                results.AddRange(response.entries.Select(x => x.name).ToList());
            }

            return results;
        }

        public async Task<byte[]> GetFileContentAsync(string path, CancellationToken cancellationToken)
        {
            var uri = new Uri(DropboxContentUri + "/2/files/download");

            var headers = new Dictionary<string, string>
                          {
                              {"Dropbox-API-Arg", $@"{{""path"":""{path}""}}"}
                          };

            return await _httpClientHelper.GetBytesAsync(uri, headers, _settings.DropboxAccessToken, cancellationToken);
        }
    }
}