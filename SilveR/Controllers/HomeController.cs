using Microsoft.AspNetCore.Mvc;
using SilveR.Models;
using System.Diagnostics;

namespace SilveR.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.InfoMessage = TempData["InfoMessage"];

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
