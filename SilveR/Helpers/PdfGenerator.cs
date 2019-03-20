using SelectPdf;
using System;
using System.IO;

namespace SilveR.Helpers
{
    public static class PdfGenerator
    {
        public static byte[] GeneratePdf(Uri resultsUrl)
        {
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            converter.Options.KeepImagesTogether = true;
            converter.Options.KeepTextsTogether = true;

            // create a new pdf document converting a url
            PdfDocument doc = converter.ConvertUrl(resultsUrl.ToString());

            // create memory stream to save PDF
            MemoryStream pdfStream = new MemoryStream();

            // save and close pdf document
            doc.Save(pdfStream);
            doc.Close();

            //convert to bytes
            byte[] bytes = ReadFully(pdfStream);
            return bytes;
        }

        private static byte[] ReadFully(Stream theStream)
        {
            theStream.Position = 0; //make sure position reset

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = theStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}