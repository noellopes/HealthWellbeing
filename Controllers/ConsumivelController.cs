using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelController : Controller
    {
        // Lista em memória — substitui a BD
        private static List<Consumivel> _consumiveis = new()
        {
            new Consumivel
            {
                ConsumivelId = 1,
                Categoria = "Limpeza",
                Nome = "Álcool Gel",
                ZonaArmazenamento = "Armário 1",
                Fornecedores = new List<string> { "Higienix", "SafeHands" },
                Stock = 25,
                SalaId = 3
            },
            new Consumivel
            {
                ConsumivelId = 2,
                Categoria = "Escritório",
                Nome = "Canetas",
                ZonaArmazenamento = "Armário 2",
                Fornecedores = new List<string> { "OfficePlus", "Papelaria Central" },
                Stock = 120,
                SalaId = 5
            }
        };

        // GET: /Consumivel/ConsumivelRegister
        public IActionResult ConsumivelRegister()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        // LISTAR
        // /Consumivel/AdministrarConsumiveis
        public IActionResult AdministrarConsumiveis()
        {
            return View(_consumiveis);
        }

        // (GET)
        public IActionResult Delete(int id)
        {
            var a = _consumiveis.FirstOrDefault(x => x.ConsumivelId == id);
            if (a == null) return NotFound();
            return View(a);
        }

        // (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var a = _consumiveis.FirstOrDefault(x => x.ConsumivelId == id);
            if (a != null) _consumiveis.Remove(a);
            return RedirectToAction("AdministrarConsumiveis");
        }
       
        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var c = _consumiveis.FirstOrDefault(x => x.ConsumivelId == id);
            if (c == null) return NotFound();
            return View(c);
        }

        // EDITAR (POST)
        [HttpPost]
        public IActionResult Edit(Consumivel c)
        {
            var old = _consumiveis.FirstOrDefault(x => x.ConsumivelId == c.ConsumivelId);
            if (old == null) return NotFound();

            old.Nome = c.Nome;
            old.Categoria = c.Categoria;
            old.ZonaArmazenamento = c.ZonaArmazenamento;
            old.Stock = c.Stock;
            old.SalaId = c.SalaId;
            old.Fornecedores = c.Fornecedores;

            return RedirectToAction("AdministrarConsumiveis");
        }
    }
}
