using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DanClarkeBlog.Core.Helpers
{
    public class SlackNotificationTarget : INotificationTarget
    {
        private readonly Settings _settings;

        public SlackNotificationTarget(Settings settings)
        {
            _settings = settings;
        }

        public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
        {
            var payload = new { text = message };

            var json = JsonConvert.SerializeObject(payload);

            using (var client = new HttpClient())
            {
                await client.PostAsync(_settings.SlackNotificationUri, new StringContent(json, Encoding.UTF8, "application/json"));
            }
        }
    }
}