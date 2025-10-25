using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;


namespace HealthWellbeing.Controllers
{
    public class UtenteSaudeController : Controller
    {
        // GET: UtenteSaudeController
        public ActionResult Index()
        {
            return View();
        }
        /*  // GET: UtenteSaudeController/Details/
        public ActionResult Details(int id)
        {
            return View();
        }*/


        // GET: UtenteSaudeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UtenteSaudeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(UtenteSaude));
            }
            catch
            {
                return View();
            }
        }

        // GET: UtenteSaudeController/Edit
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UtenteSaudeController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(UtenteSaude));
            }
            catch
            {
                return View();
            }
        }

        // GET: UtenteSaudeController/Delete
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UtenteSaudeController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(UtenteSaude));
            }
            catch
            {
                return View();
            }
        }
    }
}
