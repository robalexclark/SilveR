using Microsoft.AspNetCore.Mvc;
using SilveR.Models;
using System;
using System.Threading.Tasks;

namespace SilveR.Controllers
{
    public class UserOptionsController : Controller
    {
        private readonly ISilveRRepository repository;

        public UserOptionsController(ISilveRRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            UserOption options = await repository.GetUserOptions();
            return View(options);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUserOptions(UserOption userOption, string submitButtonValue)
        {
            if (submitButtonValue == "save")
            {
                await repository.UpdateUserOptions(userOption);
                return RedirectToAction("Index", "Home");
            }
            else if (submitButtonValue == "reset")
            {
                await repository.ResetUserOptions();

                return RedirectToAction ("Index");
            }
            else
                throw new ArgumentException("submitButtonValue is not valid!");
        }
    }
}