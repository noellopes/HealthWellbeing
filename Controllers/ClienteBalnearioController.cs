using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class ClienteBalnearioController : Controller
    {
        // LISTA FAKE (simula BD)
        private static List<ClienteBalnearioModel> _clientes = new()
        {
            new ClienteBalnearioModel { ClienteBalnearioId = 1, Nome = "Maria Silva", Email="maria@gmail.com", Telemovel="912345678", Morada="Rua A", TipoCliente="Regular" },
            new ClienteBalnearioModel { ClienteBalnearioId = 2, Nome = "João Pereira", Email="joao@gmail.com", Telemovel="913456789", Morada="Rua B", TipoCliente="VIP" }
        };


        public IActionResult Index(string search)
        {
            var lista = _clientes;

            if (!string.IsNullOrEmpty(search))
            {
                lista = lista
                    .Where(c => c.Nome.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || c.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            lista = lista.OrderBy(c => c.Nome).ToList();

            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Create(ClienteBalnearioModel cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            cliente.ClienteBalnearioId = _clientes.Max(c => c.ClienteBalnearioId) + 1;
            _clientes.Add(cliente);

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
            existente.Nome = cliente.Nome;
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
    }
}
