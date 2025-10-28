using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class ZonasArmazenamentoController : Controller
    {
        // Lista simulada (em vez de base de dados)
        private static List<ZonaArmazenamento> zonas = new List<ZonaArmazenamento>
        {
            new ZonaArmazenamento { Id = 1, Nome = "Armazém Principal", Localizacao = "Bloco A", CapacidadeMaxima = 500 },
            new ZonaArmazenamento { Id = 2, Nome = "Sala Fria", Localizacao = "Bloco B", CapacidadeMaxima = 200 }
        };

        // GET: /ZonasArmazenamento
        public IActionResult Index()
        {
            var listaOrdenada = zonas.OrderBy(z => z.Id).ToList();
            return View(listaOrdenada);
        }

        // GET: /ZonasArmazenamento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ZonasArmazenamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ZonaArmazenamento zona)
        {
            if (ModelState.IsValid)
            {
                zona.Id = zonas.Any() ? zonas.Max(z => z.Id) + 1 : 1;
                zonas.Add(zona);

                TempData["Mensagem"] = $"Zona '{zona.Nome}' adicionada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Erro"] = "Ocorreu um erro ao adicionar a zona. Verifique os dados.";
            return View(zona);
        }

        // GET: /ZonasArmazenamento/Edit/5
        public IActionResult Edit(int id)
        {
            var zona = zonas.FirstOrDefault(z => z.Id == id);
            if (zona == null)
            {
                TempData["Erro"] = "Zona não encontrada.";
                return RedirectToAction(nameof(Index));
            }
            return View(zona);
        }

        // POST: /ZonasArmazenamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ZonaArmazenamento zona)
        {
            var existente = zonas.FirstOrDefault(z => z.Id == id);
            if (existente == null)
            {
                TempData["Erro"] = "Zona não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                existente.Nome = zona.Nome;
                existente.Localizacao = zona.Localizacao;
                existente.Descricao = zona.Descricao;
                existente.CapacidadeMaxima = zona.CapacidadeMaxima;
                existente.Ativa = zona.Ativa;

                TempData["Mensagem"] = $"Zona '{zona.Nome}' editada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Erro"] = "Ocorreu um erro ao editar a zona. Verifique os dados.";
            return View(zona);
        }

        // GET: /ZonasArmazenamento/Delete/5
        public IActionResult Delete(int id)
        {
            var zona = zonas.FirstOrDefault(z => z.Id == id);
            if (zona == null)
            {
                TempData["Erro"] = "Zona não encontrada.";
                return RedirectToAction(nameof(Index));
            }
            return View(zona);
        }

        // POST: /ZonasArmazenamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var zona = zonas.FirstOrDefault(z => z.Id == id);
            if (zona != null)
            {
                zonas.Remove(zona);
                TempData["Mensagem"] = $"Zona '{zona.Nome}' eliminada!";
            }
            else
            {
                TempData["Erro"] = "Zona não encontrada ou já foi eliminada.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /ZonasArmazenamento/Details/5
        public IActionResult Details(int id)
        {
            var zona = zonas.FirstOrDefault(z => z.Id == id);
            if (zona == null)
            {
                TempData["Erro"] = "Zona não encontrada.";
                return RedirectToAction(nameof(Index));
            }
            return View(zona);
        }
    }
}
