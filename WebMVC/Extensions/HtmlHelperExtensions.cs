using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace WebMVC.Extensions
{
    public static partial class HtmlHelperExtensions
    {
        private const string _partialViewScriptItemPrefix = "scripts_";

        [GeneratedRegex("^scripts_([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$")]
        private static partial Regex PartailSectionScriptKeyMatcher();

        public static IHtmlContent PartialSectionScripts(this IHtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items[_partialViewScriptItemPrefix + Guid.NewGuid()] = template;
            return new HtmlContentBuilder();
        }

        public static IHtmlContent RenderPartialSectionScripts(this IHtmlHelper htmlHelper)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var partialSectionScripts = htmlHelper.ViewContext.HttpContext.Items.Keys
                .Where(k => k is not null && PartailSectionScriptKeyMatcher().IsMatch(k.ToString()));
#pragma warning restore CS8604 // Possible null reference argument.
            var contentBuilder = new HtmlContentBuilder();
            foreach (var key in partialSectionScripts)
            {
                if (htmlHelper.ViewContext.HttpContext.Items[key] is Func<object, HelperResult> template)
                {
                    var writer = new StringWriter();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    template(null).WriteTo(writer, HtmlEncoder.Default);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    contentBuilder.AppendHtml(writer.ToString());
                }
            }
            return contentBuilder;
        }
    }
}