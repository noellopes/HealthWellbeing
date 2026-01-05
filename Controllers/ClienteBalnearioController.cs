using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HealthWellbeing.Controllers
{
    public class ClienteBalnearioController : Controller
    {
        // LISTA FAKE (simula BD)
        private static List<ClienteBalnearioModel> _clientes = new()
        {
            new ClienteBalnearioModel { ClienteBalnearioId = 1, NomeCompleto = "Maria Silva", Email="maria@gmail.com", Telemovel="912345678", Morada="Rua A", TipoCliente="Regular" },
            new ClienteBalnearioModel { ClienteBalnearioId = 2, NomeCompleto = "João Pereira", Email="joao@gmail.com", Telemovel="913456789", Morada="Rua B", TipoCliente="VIP" }
        };


        public IActionResult Index(string search, int page = 1)
        {
            int pageSize = 5;

            var lista = _clientes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                lista = lista.Where(c =>
                    c.NomeCompleto.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            int totalItems = lista.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var clientesPaginados = lista
                .OrderBy(c => c.NomeCompleto)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            return View(clientesPaginados);
        }


        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClienteBalnearioModel cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            cliente.ClienteBalnearioId = _clientes.Any()
            ? _clientes.Max(c => c.ClienteBalnearioId) + 1
            : 1;


            TempData["Success"] = "Cliente criado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var cliente = _clientes.FirstOrDefault(c => c.ClienteBalnearioId == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        [HttpPost]
        public IActionResult Edit(ClienteBalnearioModel cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            var existente = _clientes.First(c => c.ClienteBalnearioId == cliente.ClienteBalnearioId);
            existente.NomeCompleto = cliente.NomeCompleto;
            existente.Email = cliente.Email;
            existente.Telemovel = cliente.Telemovel;
            existente.Morada = cliente.Morada;
            existente.TipoCliente = cliente.TipoCliente;

            TempData["Success"] = "Cliente atualizado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var cliente = _clientes.FirstOrDefault(c => c.ClienteBalnearioId == id);
            if (cliente != null)
            {
                _clientes.Remove(cliente);

                TempData["Success"] = "Cliente removido com sucesso.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var cliente = _clientes.FirstOrDefault(c => c.ClienteBalnearioId == id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

    }
}
