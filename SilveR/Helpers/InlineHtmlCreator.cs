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
            ArgumentNullException.ThrowIfNull(resultsFiles);

            //get html
            List<string> htmlFiles = resultsFiles
                .Where(x => x.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                    || x.EndsWith(".htm", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (htmlFiles.Count != 1)
            {
                throw new InvalidOperationException(
                    $"Expected exactly one HTML results file, but found {htmlFiles.Count}. "
                    + $"Available result files: {FormatResultFileNames(resultsFiles)}");
            }

            string htmlFile = htmlFiles[0];

            //read in the html and reconvert any dodgy characters back
            string theHTML = File.ReadAllText(htmlFile, Encoding.UTF8); // Encoding.GetEncoding(1252));

            ArgumentFormatter argFormatter = new ArgumentFormatter();
            theHTML = argFormatter.ConvertIllegalCharactersBack(theHTML);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(theHTML);

            SanitizeHtml(document, resultsFiles);
            ValidateResultImageReferences(document, resultsFiles);
            document = InlineImages(document, resultsFiles);

            List<char> trimChars = new List<char>(Environment.NewLine.ToCharArray());
            trimChars.Add(' ');
            string inlineHtml = document.DocumentNode.OuterHtml.Trim(trimChars.ToArray());
            return inlineHtml;
        }

        private static void ValidateResultImageReferences(HtmlDocument document, List<string> resultsFiles)
        {
            List<string> missingImages = document.DocumentNode
                .Descendants("img")
                .Select(node => HtmlEntity.DeEntitize(node.GetAttributeValue("src", String.Empty)).Trim())
                .Where(source => !String.IsNullOrEmpty(source)
                    && !source.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase)
                    && !resultsFiles.Any(file => Path.GetFileName(file).Equals(
                        Path.GetFileName(source), StringComparison.OrdinalIgnoreCase)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (missingImages.Count > 0)
            {
                throw new InvalidOperationException(
                    $"HTML references image file(s) that were not produced: {String.Join(", ", missingImages)}. "
                    + $"Available result files: {FormatResultFileNames(resultsFiles)}");
            }
        }

        private static string FormatResultFileNames(IEnumerable<string> resultsFiles)
        {
            string[] fileNames = resultsFiles
                .Select(Path.GetFileName)
                .OrderBy(fileName => fileName, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return fileNames.Length == 0 ? "(none)" : String.Join(", ", fileNames);
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
                    string source = HtmlEntity.DeEntitize(node.GetAttributeValue("src", String.Empty)).Trim();
                    bool isInlineImage = source.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase);
                    bool isResultImage = IsResultImageReference(source, resultsFiles);
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
                    bool isResultImageSource = node.Name.Equals("img", StringComparison.OrdinalIgnoreCase)
                        && attributeName.Equals("src", StringComparison.OrdinalIgnoreCase)
                        && IsResultImageReference(HtmlEntity.DeEntitize(attributeValue).Trim(), resultsFiles);
                    if (attributeName.StartsWith("on", StringComparison.OrdinalIgnoreCase)
                        || (!isResultImageSource && IsUnsafeUri(attributeName, attributeValue))
                        || (attributeName.Equals("style", StringComparison.OrdinalIgnoreCase) && ContainsUnsafeStyle(attributeValue)))
                    {
                        node.Attributes.Remove(attribute);
                    }
                }
            }
        }

        private static bool IsResultImageReference(string source, List<string> resultsFiles)
        {
            return !String.IsNullOrEmpty(source)
                && resultsFiles != null
                && resultsFiles.Any(file => Path.GetFileName(file).Equals(
                    Path.GetFileName(source), StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsUnsafeUri(string attributeName, string attributeValue)
        {
            if (!attributeName.Equals("href", StringComparison.OrdinalIgnoreCase)
                && !attributeName.Equals("src", StringComparison.OrdinalIgnoreCase)
                && !attributeName.Equals("xlink:href", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string decodedValue = HtmlEntity.DeEntitize(attributeValue).Trim();
            int colonIndex = decodedValue.IndexOf(':');
            int pathIndex = decodedValue.IndexOfAny(new[] { '/', '?', '#' });
            if (colonIndex < 0 || (pathIndex >= 0 && pathIndex < colonIndex))
            {
                return false;
            }

            string scheme = new string(decodedValue
                .Take(colonIndex)
                .Where(character => !Char.IsControl(character) && !Char.IsWhiteSpace(character))
                .ToArray());

            if (attributeName.Equals("href", StringComparison.OrdinalIgnoreCase))
            {
                return !scheme.Equals("http", StringComparison.OrdinalIgnoreCase)
                    && !scheme.Equals("https", StringComparison.OrdinalIgnoreCase)
                    && !scheme.Equals("mailto", StringComparison.OrdinalIgnoreCase);
            }

            return !scheme.Equals("http", StringComparison.OrdinalIgnoreCase)
                && !scheme.Equals("https", StringComparison.OrdinalIgnoreCase)
                && !(scheme.Equals("data", StringComparison.OrdinalIgnoreCase)
                    && decodedValue.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase));
        }

        private static bool ContainsUnsafeStyle(string style)
        {
            return style.IndexOf("expression(", StringComparison.OrdinalIgnoreCase) >= 0
                || style.IndexOf("javascript:", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static HtmlDocument InlineImages(HtmlDocument document, List<string> resultsFiles)
        {
            foreach (HtmlNode d in document.DocumentNode.Descendants("img").ToList())
            {
                string src = HtmlEntity.DeEntitize(d.GetAttributeValue("src", String.Empty)).Trim();
                if (String.IsNullOrEmpty(src))
                {
                    d.Remove();
                    continue;
                }

                if (src.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string imageFile = resultsFiles.FirstOrDefault(x =>
                    Path.GetFileName(x).Equals(Path.GetFileName(src), StringComparison.OrdinalIgnoreCase));
                if (imageFile == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to inline image '{src}' because no matching result file was found. "
                        + $"Available result files: {FormatResultFileNames(resultsFiles)}");
                }

                byte[] imageBytes;
                try
                {
                    imageBytes = LoadImageAsPngBytes(imageFile);
                }
                catch (Exception ex)
                {
                    FileInfo imageInfo = new FileInfo(imageFile);
                    throw new InvalidOperationException(
                        $"Unable to inline image '{src}' from result file '{imageInfo.Name}' "
                        + $"({imageInfo.Length} bytes, last modified {imageInfo.LastWriteTimeUtc:O}).",
                        ex);
                }

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
