using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Dropbox;
using DanClarkeBlog.Core.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DanClarkeBlog.Core.Helpers
{
    // See following link for reference (and live testing) of all the API endpoints
    // https://dropbox.github.io/dropbox-api-v2-explorer/

    [UsedImplicitly]
    public class DropboxHelper : IDropboxHelper
    {
        private readonly Settings _settings;
        private readonly IHttpClientHelper _httpClientHelper;

        private static readonly string DropboxApiUri = "https://api.dropboxapi.com";
        private static readonly string DropboxContentUri = "https://content.dropboxapi.com";

        public DropboxHelper(IOptions<Settings> settings, IHttpClientHelper httpClientHelper)
        {
            _settings = settings.Value;
            _httpClientHelper = httpClientHelper;
        }

        public Task<List<DropboxFileModel>> GetFilesAsync(string path, CancellationToken cancellationToken)
        {
            return GetFilesAsync(path, null, cancellationToken);
        }

        public async Task<List<DropboxFileModel>> GetFilesAsync(string path, CursorContainer cursor, CancellationToken cancellationToken)
        {
            var uri = new Uri(DropboxApiUri + "/2/files/list_folder");
            var continueUri = new Uri(DropboxApiUri + "/2/files/list_folder/continue");

            List<DropboxFileModel> results;
            DropboxApiResponseListFiles response;

            if (string.IsNullOrWhiteSpace(cursor?.Cursor))
            {
                var jsonResponse = await _httpClientHelper.PostAsync(uri, $@"{{""path"":""/Blog{path}"", ""recursive"": true}}", _settings.DropboxAccessToken, cancellationToken);

                response = JsonConvert.DeserializeObject<DropboxApiResponseListFiles>(jsonResponse);

                results = response.entries.Select(x => new DropboxFileModel(x.name, x.path_lower)).ToList();
            }
            else
            {
                results = new List<DropboxFileModel>();
                response = new DropboxApiResponseListFiles {cursor = cursor.Cursor, has_more = true};
            }

            while (response.has_more)
            {
                var json = $@"{{""cursor"": ""{response.cursor}""}}";
                var jsonResponse = await _httpClientHelper.PostAsync(continueUri, json, _settings.DropboxAccessToken, cancellationToken);
                response = JsonConvert.DeserializeObject<DropboxApiResponseListFiles>(jsonResponse);

                if (response.entries != null)
                {
                    results.AddRange(response.entries.Select(x => new DropboxFileModel(x.name, x.path_lower)).ToList());
                }

	            if (cursor != null)
	            {
		            cursor.Cursor = response.cursor;
	            }
            }

            return results;
        }

        public async Task<byte[]> GetFileContentAsync(string path, CancellationToken cancellationToken)
        {
            var uri = new Uri(DropboxContentUri + "/2/files/download");

            var headers = new Dictionary<string, string>
                          {
                              {"Dropbox-API-Arg", $@"{{""path"":""/Blog{path}""}}"}
                          };

            return await _httpClientHelper.GetBytesAsync(uri, headers, _settings.DropboxAccessToken, cancellationToken);
        }

        public async Task<string> GetCurrentCursorAsync(CancellationToken cancellationToken)
        {
            var uri = new Uri(DropboxApiUri + "/2/files/list_folder/get_latest_cursor");

            var jsonResponse = await _httpClientHelper.PostAsync(uri, @"{""path"":""/Blog"", ""recursive"": true}", _settings.DropboxAccessToken, cancellationToken);

            var response = JsonConvert.DeserializeObject<DropboxApiResponseGetCurrentCursor>(jsonResponse);

            return response.cursor;
        }
    }
}