using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SilveR.IntegrationTests
{
    public static class Helpers
    {
        public static async Task<IEnumerable<string>> ExtractErrors(HttpResponseMessage response)
        {
            var html = await response.Content.ReadAsStringAsync();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            List<string> result = new List<string>();

            HtmlNode errorsNode = htmlDocument.GetElementbyId("errorsList");
            if (errorsNode != null)
            {
                string errors = errorsNode.InnerText;
                result.AddRange(errors.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList());
            }

            HtmlNode warningsNode = htmlDocument.GetElementbyId("warningsList");
            if (warningsNode != null)
            {
                string warnings = warningsNode.InnerText;
                result.AddRange(warnings.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList());
            }

            return result;
        }
    }
}