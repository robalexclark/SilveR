using HtmlAgilityPack;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SilveR.Helpers
{
    public static class InlineHtmlCreator
    {
        public static string CreateInlineHtml(List<string> resultsFiles)
        {
            //get html
            string htmlFile = resultsFiles.Single(x => x.EndsWith(".html") || x.EndsWith(".htm"));

            //read in the html and reconvert any dodgy characters back
            string theHTML = File.ReadAllText(htmlFile, Encoding.UTF8); // Encoding.GetEncoding(1252));

            ArgumentFormatter argFormatter = new ArgumentFormatter();
            theHTML = argFormatter.ConvertIllegalCharactersBack(theHTML);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(theHTML);

            SanitizeHtml(document, resultsFiles);
            document = InlineImages(document, resultsFiles);

            List<char> trimChars = new List<char>(Environment.NewLine.ToCharArray());
            trimChars.Add(' ');
            string inlineHtml = document.DocumentNode.OuterHtml.Trim(trimChars.ToArray());
            return inlineHtml;
        }

        public static string SanitizeStoredHtml(string html)
        {
            if (String.IsNullOrEmpty(html))
            {
                return String.Empty;
            }

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            SanitizeHtml(document, null);
            return document.DocumentNode.OuterHtml;
        }

        private static void SanitizeHtml(HtmlDocument document, List<string> resultsFiles)
        {
            string[] unsafeElements = { "script", "iframe", "object", "embed", "svg", "math", "base", "meta" };
            foreach (HtmlNode node in document.DocumentNode.Descendants().Where(x => unsafeElements.Contains(x.Name, StringComparer.OrdinalIgnoreCase)).ToList())
            {
                node.Remove();
            }

            foreach (HtmlNode node in document.DocumentNode.Descendants().ToList())
            {
                if (node.Name.Equals("img", StringComparison.OrdinalIgnoreCase))
                {
                    string source = node.GetAttributeValue("src", String.Empty);
                    bool isInlineImage = source.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase);
                    bool isResultImage = resultsFiles != null && resultsFiles.Any(x => Path.GetFileName(x).Equals(Path.GetFileName(source), StringComparison.OrdinalIgnoreCase));
                    if (!isInlineImage && !isResultImage)
                    {
                        node.Remove();
                        continue;
                    }
                }

                foreach (HtmlAttribute attribute in node.Attributes.ToList())
                {
                    string attributeName = attribute.Name;
                    string attributeValue = attribute.Value?.Trim() ?? String.Empty;
                    if (attributeName.StartsWith("on", StringComparison.OrdinalIgnoreCase)
                        || IsUnsafeUri(attributeName, attributeValue)
                        || (attributeName.Equals("style", StringComparison.OrdinalIgnoreCase) && ContainsUnsafeStyle(attributeValue)))
                    {
                        node.Attributes.Remove(attribute);
                    }
                }
            }
        }

        private static bool IsUnsafeUri(string attributeName, string attributeValue)
        {
            if (!attributeName.Equals("href", StringComparison.OrdinalIgnoreCase)
                && !attributeName.Equals("src", StringComparison.OrdinalIgnoreCase)
                && !attributeName.Equals("xlink:href", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return attributeValue.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase)
                || attributeValue.StartsWith("vbscript:", StringComparison.OrdinalIgnoreCase)
                || attributeValue.StartsWith("data:text/html", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsUnsafeStyle(string style)
        {
            return style.IndexOf("expression(", StringComparison.OrdinalIgnoreCase) >= 0
                || style.IndexOf("javascript:", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static HtmlDocument InlineImages(HtmlDocument document, List<string> resultsFiles)
        {
            foreach (HtmlNode d in document.DocumentNode.Descendants("img"))
            {
                string src = d.GetAttributeValue("src", null);

                string imageFile = resultsFiles.Single(x => Path.GetFileName(x) == Path.GetFileName(src));

                byte[] imageBytes = LoadImageAsPngBytes(imageFile);

                // Convert byte[] to Base64 String so the image can be embedded inline
                string base64String = Convert.ToBase64String(imageBytes);

                d.SetAttributeValue("src", "data:image/png;base64," + base64String);
            }

            return document;
        }

        private const int MaxImageLoadAttempts = 5;
        private const int ImageLoadRetryDelayMs = 100;

        private static byte[] LoadImageAsPngBytes(string imageFile)
        {
            for (int attempt = 0; attempt < MaxImageLoadAttempts; attempt++)
            {
                try
                {
                    using (FileStream stream = new FileStream(imageFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (Image image = Image.Load(stream))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.SaveAsPng(ms);
                        return ms.ToArray();
                    }
                }
                catch (IOException) when (attempt < MaxImageLoadAttempts - 1)
                {
                    Thread.Sleep(ImageLoadRetryDelayMs);
                }
                catch (UnauthorizedAccessException) when (attempt < MaxImageLoadAttempts - 1)
                {
                    Thread.Sleep(ImageLoadRetryDelayMs);
                }
            }

            throw new IOException($"Unable to load image '{imageFile}' after {MaxImageLoadAttempts} attempts.");
        }
    }
}
