using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SilveR.Models;
using System.Diagnostics;
using System.IO;

namespace SilveR.Controllers
{
    public class HomeController : Controller
    {
        private readonly string wwwRoot;
        public HomeController(IHostingEnvironment env)
        {
            wwwRoot = env.WebRootPath;
        }
        public IActionResult Index()
        {
            return View();
        }

        public void OpenExternalUrl(string externalUrl)
        {
            Electron.Shell.OpenExternalAsync(externalUrl);

            //return View("Index");
        }

        public void OpenItem(string itemPath)
        {
            if(itemPath.StartsWith("http:"))
            {
                Electron.Shell.OpenExternalAsync(itemPath);
            }
            else
            {
                Electron.Shell.OpenItemAsync(Path.Combine(wwwRoot, itemPath));
            }

            //return View("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}