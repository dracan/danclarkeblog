using DanClarkeBlog.Core;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace DanClarkeBlog.Web.TagHelpers
{
    [UsedImplicitly]
    public class GoogleAnalyticsTagHelper : TagHelper
    {
        private readonly Settings _settings;

        public GoogleAnalyticsTagHelper(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            output.Content.SetHtmlContent($@"
                <script>
                  (function(i,s,o,g,r,a,m){{i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){{
                  (i[r].q=i[r].q||[]).push(arguments)}},i[r].l=1*new Date();a=s.createElement(o),
                  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
                  }})(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

                  ga('create', '{_settings.GoogleAnalyticsTrackingId}', 'auto');
                  ga('send', 'pageview');

                </script>
            ");
        }
    }
}