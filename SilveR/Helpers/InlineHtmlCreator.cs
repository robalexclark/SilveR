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

            document = InlineImages(document, resultsFiles);

            List<char> trimChars = new List<char>(Environment.NewLine.ToCharArray());
            trimChars.Add(' ');
            string inlineHtml = document.DocumentNode.OuterHtml.Trim(trimChars.ToArray());
            return inlineHtml;
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
