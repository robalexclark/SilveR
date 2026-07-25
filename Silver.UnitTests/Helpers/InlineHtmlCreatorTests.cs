using SilveR.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SilveR.UnitTests.Helpers
{
    public class InlineHtmlCreatorTests
    {
        [Fact]
        public void CreateInlineHtml_UnsafeMarkupInResults_RemovesExecutableContent()
        {
            string testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDirectory);

            try
            {
                string htmlPath = Path.Combine(testDirectory, "results.html");
                string imagePath = Path.Combine(testDirectory, "results.png");
                File.WriteAllText(htmlPath, "<html><body><p>Fixed factors: ivs_lt_ivsscriptivs_gt_ivsalert(1)ivs_lt_ivs/ scriptivs_gt_ivs</p><img src=\"results.png\" onerror=\"alert(1)\" /><a href=\"javascript:alert(1)\">unsafe</a></body></html>");
                File.WriteAllBytes(imagePath, Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAF/gL+qM1hTAAAAABJRU5ErkJggg=="));

                string result = InlineHtmlCreator.CreateInlineHtml(new List<string> { htmlPath, imagePath });

                Assert.DoesNotContain("<script", result, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("onerror", result, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("javascript:", result, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [Fact]
        public void SanitizeStoredHtml_UnsafeMarkup_RemovesExecutableContentAndKeepsInlineImage()
        {
            string result = InlineHtmlCreator.SanitizeStoredHtml("<p>Result</p><script>alert(1)</script><img src=\"data:image/png;base64,AA==\" onerror=\"alert(1)\" /><a href=\"javascript:alert(1)\">unsafe</a>");

            Assert.DoesNotContain("<script", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("onerror", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("javascript:", result, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("data:image/png;base64,AA==", result, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("&#106;avascript:alert(1)")]
        [InlineData("java&#x09;script:alert(1)")]
        [InlineData("vbscript:alert(1)")]
        [InlineData("data:text/html,&lt;script&gt;alert(1)&lt;/script&gt;")]
        public void SanitizeStoredHtml_EncodedOrDisallowedUri_RemovesAttribute(string uri)
        {
            string result = InlineHtmlCreator.SanitizeStoredHtml($"<a href=\"{uri}\">unsafe</a>");

            Assert.DoesNotContain("href=", result, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("/analyses/result")]
        [InlineData("#result")]
        [InlineData("https://example.test/result")]
        [InlineData("mailto:analyst@example.test")]
        public void SanitizeStoredHtml_AllowedUri_KeepsAttribute(string uri)
        {
            string result = InlineHtmlCreator.SanitizeStoredHtml($"<a href=\"{uri}\">safe</a>");

            Assert.Contains("href=", result, StringComparison.OrdinalIgnoreCase);
        }
    }
}
