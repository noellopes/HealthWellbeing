using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class UnidadeController : Controller
    {
        // GET: UnidadeController
        public ActionResult Unidade()
        {
            return View();
        }

        // GET: UnidadeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UnidadeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UnidadeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Unidade));
            }
            catch
            {
                return View();
            }
        }

        // GET: UnidadeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UnidadeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Unidade));
            }
            catch
            {
                return View();
            }
        }

        // GET: UnidadeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UnidadeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Unidade));
            }
            catch
            {
                return View();
            }
        }
    }
}
