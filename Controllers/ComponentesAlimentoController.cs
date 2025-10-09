using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ComponentesAlimentoController : Controller
    {
        // GET: ComponentesAlimentoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ComponentesAlimentoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ComponentesAlimentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComponentesAlimentoController/Create
        [HttpPost]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(ComponentesAlimentos));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComponentesAlimentoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComponentesAlimentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(ComponentesAlimentos));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComponentesAlimentoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ComponentesAlimentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(ComponentesAlimentos));
            }
            catch
            {
                return View();
            }
        }
    }
}
