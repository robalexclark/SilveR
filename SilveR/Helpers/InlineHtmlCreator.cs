﻿using HtmlAgilityPack;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

                using (var image = Image.Load(imageFile))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.SaveAsPng(ms);
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