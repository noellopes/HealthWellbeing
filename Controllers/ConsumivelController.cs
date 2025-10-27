using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelController : Controller
    {
        // Lista em memória
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

        // VIEW: Listar os consumíveis
        public IActionResult AdministrarConsumiveis()
        {
            return View(_consumiveis);
        }

        // CRIAR (GET)
        public IActionResult ConsumivelRegister() => View();

        // CRIAR (POST)
        [HttpPost]
        public IActionResult ConsumivelRegister(Consumivel consumivel, string FornecedoresTexto)
        {
            // Converte fornecedores de texto em lista
            consumivel.Fornecedores = !string.IsNullOrEmpty(FornecedoresTexto)
                ? FornecedoresTexto.Split(',').Select(f => f.Trim()).ToList()
                : new List<string>();

            // Atribui ID automático
            consumivel.ConsumivelId = _consumiveis.Any() ? _consumiveis.Max(c => c.ConsumivelId) + 1 : 1;

            // Adiciona à lista
            _consumiveis.Add(consumivel);

            // Redireciona para a lista
            return RedirectToAction("AdministrarConsumiveis");
        }

        // ELIMINAR (GET)
        public IActionResult Delete(int id)
        {
            var c = _consumiveis.FirstOrDefault(x => x.ConsumivelId == id);
            if (c == null) return NotFound();
            return View(c);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var c = _consumiveis.FirstOrDefault(x => x.ConsumivelId == id);
            if (c != null) _consumiveis.Remove(c);
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
