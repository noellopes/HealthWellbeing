using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class AlimentoController : Controller
    {
        // GET: AlimentoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AlimentoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AlimentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AlimentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AlimentoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AlimentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AlimentoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AlimentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
