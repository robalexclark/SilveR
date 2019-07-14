using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using SilveR.StatsModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilveR.IntegrationTests
{
    public static class Helpers
    {
        public static async Task<IEnumerable<string>> ExtractErrors(HttpResponseMessage response)
        {
            string html = await response.Content.ReadAsStringAsync();
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
            string html = await response.Content.ReadAsStringAsync();
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

        private static string Fix(string s)
        {
            s = s.Replace("&lt;", "<");
            s = s.Replace("&gt;", ">");

            return s;
        }

        public static void SaveOutput(string moduleName, string testName, IEnumerable<string> message)
        {
            Directory.CreateDirectory("ActualResults");
            Directory.CreateDirectory(Path.Combine("ActualResults", moduleName));

            File.WriteAllLines(Path.Combine("ActualResults", moduleName, testName + ".txt"), message);
        }

        public static void SaveTestOutput(string moduleName, AnalysisModelBase analysisModel, string testName, StatsOutput statsOutput)
        {
            Directory.CreateDirectory("ActualResults");
            Directory.CreateDirectory(Path.Combine("ActualResults", moduleName));

            File.WriteAllText(Path.Combine("ActualResults", moduleName, testName + ".html"), statsOutput.HtmlResults);

            List<string> logOutput = new List<string>();

            logOutput.Add(ObjectDumper.Dump(analysisModel));
            logOutput.Add("------------------------------------------------------------------------------------------------------------------");
            logOutput.Add(statsOutput.LogText);

            File.WriteAllLines(Path.Combine("ActualResults", moduleName, testName + ".log"), logOutput);
        }

        public static async Task<StatsOutput> SubmitAnalysis(HttpClient client, string analysisName, FormUrlEncodedContent content)
        {
            HttpResponseMessage response = await client.PostAsync("Analyses/" + analysisName, content);
            HtmlDocument doc = await GetHtml(response);
            string script = doc.DocumentNode.Descendants().Last(n => n.Name == "script").InnerText;

            if (String.IsNullOrEmpty(script))
            {
                throw new InvalidOperationException("script is null - warnings/errors present?");
            }
            else if (doc.ParsedText.Contains("An unhandled exception occurred while processing the request."))
            {
                string error = doc.DocumentNode.Descendants().First(x => x.HasClass("titleerror")).InnerText;
                throw new InvalidOperationException(error);
            }

            string analysisGuid = script.Split("\"", StringSplitOptions.RemoveEmptyEntries)[1];

            string jsonResponse = null;
            while (jsonResponse != "true")
            {
                Thread.Sleep(1000);
                HttpResponseMessage completedResponse = await client.PostAsync("/Analyses/AnalysisCompleted?analysisGuid=" + analysisGuid, null);
                jsonResponse = await completedResponse.Content.ReadAsStringAsync();
            }

            HttpResponseMessage viewResultsResponse = await client.GetAsync("/Analyses/ViewResults?analysisGuid=" + analysisGuid);

            HtmlDocument htmlResultsOutputDocument = await GetHtml(viewResultsResponse);
            HtmlNode resultsOutputNode = htmlResultsOutputDocument.GetElementbyId("resultsOutput");

            if (resultsOutputNode == null)
                throw new Exception("No results output!");

            string rawHtml = resultsOutputNode.InnerHtml;
            string formattedHtml = File.ReadAllText("ResultFormatting.txt") + rawHtml;


            HttpResponseMessage viewLogOutput = await client.GetAsync("/Analyses/ViewLog?analysisGuid=" + analysisGuid);
            HtmlDocument htmlLogOutputDocument = await GetHtml(viewLogOutput);
            HtmlNode logOutputNode = htmlLogOutputDocument.GetElementbyId("logOutput");

            StatsOutput statsOutput = new StatsOutput(formattedHtml, logOutputNode.InnerText);
            return statsOutput;
        }

        private static async Task<HtmlDocument> GetHtml(HttpResponseMessage response)
        {
            string html = await response.Content.ReadAsStringAsync();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        public static string RemoveAllImageNodes(string html)
        {
            try
            {
                HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(html);

                HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//img");

                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        node.Attributes.Remove("src"); //This only removes the src Attribute from <img> tag
                    }
                }

                html = document.DocumentNode.OuterHtml;
                return html;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    public class StatsOutput
    {
        private readonly string htmlResults;
        public string HtmlResults { get { return htmlResults; } }

        private readonly string logText;
        public string LogText { get { return logText; } }

        public StatsOutput(string htmlResults, string logText)
        {
            this.htmlResults = htmlResults;
            this.logText = logText;
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


    public class ObjectDumper
    {
        private int _level;
        private readonly int _indentSize;
        private readonly StringBuilder _stringBuilder;
        private readonly List<int> _hashListOfFoundElements;

        private ObjectDumper(int indentSize)
        {
            _indentSize = indentSize;
            _stringBuilder = new StringBuilder();
            _hashListOfFoundElements = new List<int>();
        }

        public static string Dump(object element)
        {
            return Dump(element, 4);
        }

        public static string Dump(object element, int indentSize)
        {
            var instance = new ObjectDumper(indentSize);
            return instance.DumpElement(element);
        }

        private string DumpElement(object element)
        {
            if (element == null || element is ValueType || element is string)
            {
                Write(FormatValue(element));
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    Write("{{{0}}}", objectType.FullName);
                    _hashListOfFoundElements.Add(element.GetHashCode());
                    _level++;
                }

                var enumerableElement = element as IEnumerable;
                if (enumerableElement != null)
                {
                    foreach (object item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            _level++;
                            DumpElement(item);
                            _level--;
                        }
                        else
                        {
                            if (!AlreadyTouched(item))
                                DumpElement(item);
                            else
                                Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName);
                        }
                    }
                }
                else
                {
                    MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var memberInfo in members)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        var propertyInfo = memberInfo as PropertyInfo;

                        if (fieldInfo == null && propertyInfo == null)
                            continue;

                        var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                        object value = fieldInfo != null
                                           ? fieldInfo.GetValue(element)
                                           : propertyInfo.GetValue(element, null);

                        if (type.IsValueType || type == typeof(string))
                        {
                            Write("{0}: {1}", memberInfo.Name, FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                            Write("{0}: {1}", memberInfo.Name, isEnumerable ? ":" : "{ }");

                            var alreadyTouched = !isEnumerable && AlreadyTouched(value);
                            _level++;
                            if (!alreadyTouched)
                                DumpElement(value);
                            else
                                Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            _level--;
                        }
                    }
                }

                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    _level--;
                }
            }

            return _stringBuilder.ToString();
        }

        private bool AlreadyTouched(object value)
        {
            if (value == null)
                return false;

            var hash = value.GetHashCode();
            for (var i = 0; i < _hashListOfFoundElements.Count; i++)
            {
                if (_hashListOfFoundElements[i] == hash)
                    return true;
            }
            return false;
        }

        private void Write(string value, params object[] args)
        {
            var space = new string(' ', _level * _indentSize);

            if (args != null)
                value = string.Format(value, args);

            _stringBuilder.AppendLine(space + value);
        }

        private string FormatValue(object o)
        {
            if (o == null)
                return ("null");

            if (o is DateTime)
                return (((DateTime)o).ToShortDateString());

            if (o is string)
                return string.Format("\"{0}\"", o);

            if (o is char && (char)o == '\0')
                return string.Empty;

            if (o is ValueType)
                return (o.ToString());

            if (o is IEnumerable)
                return ("...");

            return ("{ }");
        }
    }
}