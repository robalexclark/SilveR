using HtmlAgilityPack;
using SilveR.Helpers;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SilveR.Helpers
{
    public static class InlineHtmlCreator
    {
        public static string CreateInlineHtml(List<string> resultsFiles)
        {
            //get html
            string htmlFile = resultsFiles.Single(x => x.EndsWith(".html") || x.EndsWith(".htm"));

            //read in the html and reconvert any dodgy characters back
            string theHTML = File.ReadAllText(htmlFile);

            ArgumentFormatter argFormatter = new ArgumentFormatter();
            theHTML = argFormatter.ConvertIllegalCharactersBack(theHTML);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(theHTML);

            document = InlineImages(document, resultsFiles);

            string inlineHtml = document.DocumentNode.OuterHtml;
            return inlineHtml;
        }

        //private static HtmlDocument InlineCSS(HtmlDocument document, string cssString)
        //{
        //    HtmlNode linkNode = document.DocumentNode.SelectSingleNode("link");

        //    HtmlNode cssNode = HtmlNode.CreateNode("<style scoped>");
        //    cssNode.InnerHtml = cssString;

        //    document.DocumentNode.ReplaceChild(cssNode, linkNode);
        //    return document;
        //}

        private static HtmlDocument InlineImages(HtmlDocument document, List<string> resultsFiles)
        {
            foreach (HtmlNode d in document.DocumentNode.Descendants("img"))
            {
                string src = d.GetAttributeValue("src", null);

                string imageFile = resultsFiles.Single(x => Path.GetFileName(x) == Path.GetFileName(src));

                using (var image = Image.Load(imageFile))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormats.Png);
                        byte[] imageBytes = ms.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);

                        d.SetAttributeValue("src", "data:image/png;base64," + base64String);
                    }
                }
            }

            return document;
        }
    }
}