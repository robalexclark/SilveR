using HtmlAgilityPack;
using SilveRModel.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SilveR.Services
{
    public static class InlineHtmlCreator
    {
        public static string CreateInlineHtml(List<string> resultsFiles)
        {
            //get html
            string htmlFile = resultsFiles.Single(x => x.EndsWith(".html") || x.EndsWith(".htm"));

            //read in the html and reconvert any dodhy characters back
            string theHTML = File.ReadAllText(htmlFile);
            theHTML = ArgumentConverters.ConvertIVSCharactersBack(theHTML);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(theHTML);

            document = InlineImages(document, resultsFiles);

            string inlineHtml = document.DocumentNode.OuterHtml;
            return inlineHtml;
        }

        private static HtmlDocument InlineCSS(HtmlDocument document, string cssString)
        {
            HtmlNode linkNode = document.DocumentNode.SelectSingleNode("link");

            HtmlNode cssNode = HtmlNode.CreateNode("<style scoped>");
            cssNode.InnerHtml = cssString;

            document.DocumentNode.ReplaceChild(cssNode, linkNode);

            return document;
        }

        private static HtmlDocument InlineImages(HtmlDocument document, List<string> resultsFiles)
        {
            foreach (HtmlNode d in document.DocumentNode.Descendants("img"))
            {
                string src = d.GetAttributeValue("src", null);

                string imageFile = resultsFiles.Single(x => Path.GetFileName(x) == Path.GetFileName(src));

                using (Image image = Image.FromFile(imageFile))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

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