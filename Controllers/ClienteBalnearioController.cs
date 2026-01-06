using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class ClienteBalnearioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClienteBalnearioController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, int page = 1)
        {
            int pageSize = 5;

            var query = _context.ClienteBalneario.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.NomeCompleto.Contains(search) ||
                    c.Email.Contains(search));
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var clientesPaginados = query
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

            _context.ClienteBalneario.Add(cliente);
            _context.SaveChanges();

            TempData["Success"] = "Cliente criado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var cliente = _context.ClienteBalneario.Find(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ClienteBalnearioModel cliente)
        {
            if (id != cliente.ClienteBalnearioId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(cliente);

            var clienteDb = _context.ClienteBalneario.Find(id);
            if (clienteDb == null)
                return NotFound();

            // Atualizar campos manualmente (MELHOR PRÁTICA)
            clienteDb.NomeCompleto = cliente.NomeCompleto;
            clienteDb.Email = cliente.Email;
            clienteDb.Telemovel = cliente.Telemovel;
            clienteDb.Morada = cliente.Morada;
            clienteDb.TipoCliente = cliente.TipoCliente;

            _context.SaveChanges();

            TempData["Success"] = "Cliente atualizado com sucesso.";

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var cliente = _context.ClienteBalneario.Find(id);
            if (cliente == null)
                return NotFound();

            _context.ClienteBalneario.Remove(cliente);
            _context.SaveChanges();

            TempData["Success"] = "Cliente removido com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var cliente = _context.ClienteBalneario.Find(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }
    }
}
