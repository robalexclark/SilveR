using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
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
                IEnumerable<string> errors = errorsNode.InnerText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                foreach (string e in errors)
                {
                    if (!String.IsNullOrWhiteSpace(e))
                        result.Add(Fix(e.Trim()));
                }
            }

            return result;
        }

        public static async Task<IEnumerable<string>> ExtractWarnings(HttpResponseMessage response)
        {
            var html = await response.Content.ReadAsStringAsync();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            List<string> result = new List<string>();

            HtmlNode warningsNode = htmlDocument.GetElementbyId("warningsList");
            if (warningsNode != null)
            {
                IEnumerable<string> warnings = warningsNode.InnerText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                foreach (string w in warnings)
                {
                    if (!String.IsNullOrWhiteSpace(w))
                        result.Add(Fix(w.Trim()));
                }
            }

            return result;
        }
               
        private static string Fix(string v)
        {
            v = v.Replace("&lt;", "<");
            v = v.Replace("&gt;", ">");

            return v;
        }

        public static void SaveOutput(string moduleName, string testName, IEnumerable<string> message)
        {
            Directory.CreateDirectory("ActualResults");
            Directory.CreateDirectory(Path.Combine("ActualResults", moduleName));

            File.WriteAllLines(Path.Combine("ActualResults", moduleName, testName + ".txt"), message);
        }

        public static void SaveHtmlOutput(string moduleName, string testName, string htmlOutput)
        {
            Directory.CreateDirectory("ActualResults");
            Directory.CreateDirectory(Path.Combine("ActualResults", moduleName));

            File.WriteAllText(Path.Combine("ActualResults", moduleName, testName + ".html"), htmlOutput);
        }

        public static async Task<string> SubmitAnalysis(HttpClient client, string analysisName, FormUrlEncodedContent content)
        {
            HttpResponseMessage response = await client.PostAsync("Analyses/" + analysisName, content);
            HtmlDocument doc = await GetHtml(response);
            string script = doc.DocumentNode.Descendants().Last(n => n.Name == "script").InnerText;

            if (String.IsNullOrEmpty(script))
                throw new InvalidOperationException("script is null - warnings/errors present?");

            string analysisGuid = script.Split("\"", StringSplitOptions.RemoveEmptyEntries)[1];

            string jsonResponse = null;
            DateTime timoutTime = DateTime.Now.AddSeconds(30);
            while (jsonResponse != "true" && DateTime.Now <= timoutTime)
            {
                Thread.Sleep(1000);
                HttpResponseMessage completedResponse = await client.PostAsync("/Analyses/AnalysisCompleted?analysisGuid=" + analysisGuid, null);
                jsonResponse = await completedResponse.Content.ReadAsStringAsync();
            }

            HttpResponseMessage viewResultsResponse = await client.GetAsync("/Analyses/ViewResults?analysisGuid=" + analysisGuid);

            HtmlDocument htmlResultsOutputDocument = await GetHtml(viewResultsResponse);
            HtmlNode resultsOutputNode = htmlResultsOutputDocument.GetElementbyId("resultsOutput");

            string rawHtml = resultsOutputNode.InnerHtml;
            string formattedHtml = File.ReadAllText("ResultFormatting.txt") + rawHtml;

            return formattedHtml;
        }

        private static async Task<HtmlDocument> GetHtml(HttpResponseMessage response)
        {
            var html = await response.Content.ReadAsStringAsync();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
    }



    public static class ObjectExtensions
    {
        public static IDictionary<string, string> ToKeyValue(this object metaToken)
        {
            if (metaToken == null)
            {
                return null;
            }

            JToken token = metaToken as JToken;
            if (token == null)
            {
                return ToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = child.ToKeyValue();
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent)
                            .ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return null;
            }

            var value = jValue?.Type == JTokenType.Date ?
                jValue?.ToString("o", CultureInfo.InvariantCulture) :
                jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }
    }
}