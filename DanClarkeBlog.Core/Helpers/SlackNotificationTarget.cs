using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DanClarkeBlog.Core.Helpers
{
    [UsedImplicitly]
    public class SlackNotificationTarget : INotificationTarget
    {
        private readonly Settings _settings;

        public SlackNotificationTarget(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
        {
            var payload = new { text = message };

            var json = JsonConvert.SerializeObject(payload);

            using (var client = new HttpClient())
            {
                await client.PostAsync(_settings.SlackNotificationUri, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);
            }
        }
    }
}