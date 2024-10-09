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

            output.Content.SetHtmlContent(@$"
            <!-- Google tag (gtag.js) -->
            <script async src=""https://www.googletagmanager.com/gtag/js?id=G-L73V611F4N""></script>
            <script>
            window.dataLayer = window.dataLayer || [];
            function gtag(){{dataLayer.push(arguments);}}
            gtag('js', new Date());

            gtag('config', '{_settings.GoogleTagId}');
            </script>
            ");
        }
    }
}