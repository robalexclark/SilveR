using Microsoft.AspNetCore.Mvc;
using SilveR.Models;
using System.Diagnostics;
using System.IO;

namespace SilveR.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public FileContentResult DownloadUserGuide()
        {
            string filename = "IVS startup wizard.pptx";
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                // Create a byte array of file stream length
                byte[] bytes = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(bytes, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();

                Response.Headers.Add("Content-Disposition", "inline; filename=IVS startup wizard.ppt");
                return File(bytes, "application/pptx");
            }
        }
    }
}