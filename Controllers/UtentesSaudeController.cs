using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;


namespace HealthWellbeing.Controllers
{
    public class UtentesSaudeController : Controller
    {
        // GET: UtentesSaudeController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UtentesSaudeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UtentesSaudeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UtentesSaudeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(UtentesSaude));
            }
            catch
            {
                return View();
            }
        }

        // GET: UtentesSaudeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UtentesSaudeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(UtentesSaude));
            }
            catch
            {
                return View();
            }
        }

        // GET: UtentesSaudeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UtentesSaudeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(UtentesSaude));
            }
            catch
            {
                return View();
            }
        }
    }
}
