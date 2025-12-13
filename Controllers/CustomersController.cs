using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers {
    public class CustomersController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public CustomersController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchEmail = "", int? searchLevelId = null) {
            // Preparar Query com Eager Loading (Level) para mostrar na lista
            var customersQuery = _context.Customer
                .Include(c => c.Level)
                .AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(searchName)) {
                customersQuery = customersQuery.Where(c => c.Name.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchEmail)) {
                customersQuery = customersQuery.Where(c => c.Email.Contains(searchEmail));
            }

            if (searchLevelId.HasValue) {
                customersQuery = customersQuery.Where(c => c.LevelId == searchLevelId);
            }

            // Manter estado dos filtros na View
            ViewBag.SearchName = searchName;
            ViewBag.SearchEmail = searchEmail;

            // Dropdown para filtro de Nível (Ordenado por número)
            ViewData["Levels"] = new SelectList(_context.Level.OrderBy(l => l.LevelNumber), "LevelId", "LevelNumber", searchLevelId);

            // Paginação
            int totalItems = await customersQuery.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<Customer>(page, totalItems);

            paginationInfo.Items = await customersQuery
                .OrderBy(c => c.Name)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var customer = await _context.Customer
                .Include(c => c.Level)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null) {
                return NotFound(); // Ou View("InvalidCustomer")
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create() {
            // Carregar Dropdown de Níveis
            ViewData["LevelId"] = new SelectList(_context.Level.OrderBy(l => l.LevelNumber), "LevelId", "LevelNumber");
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Name,Email,PhoneNumber,RegistrationDate,Gender,TotalPoints,LevelId")] Customer customer) {
            // Validação customizada de Email único (Cliente ou Funcionário)
            if (ModelState.IsValid) {
                if (await _context.Customer.AnyAsync(c => c.Email == customer.Email) ||
                    await _context.Employee.AnyAsync(e => e.Email == customer.Email)) {
                    ModelState.AddModelError(nameof(Customer.Email), "Email is already in use.");
                }
            }

            if (ModelState.IsValid) {
                _context.Add(customer);
                await _context.SaveChangesAsync();

                // Redirecionar para Details com mensagem de sucesso (Padrão EventTypes)
                return RedirectToAction(nameof(Details),
                    new {
                        id = customer.CustomerId,
                        SuccessMessage = "Customer created successfully."
                    }
                );
            }

            // Recarregar Dropdown em caso de erro
            ViewData["LevelId"] = new SelectList(_context.Level.OrderBy(l => l.LevelNumber), "LevelId", "LevelNumber", customer.LevelId);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null) {
                return NotFound();
            }

            ViewData["LevelId"] = new SelectList(_context.Level.OrderBy(l => l.LevelNumber), "LevelId", "LevelNumber", customer.LevelId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Name,Email,PhoneNumber,RegistrationDate,Gender,TotalPoints,LevelId")] Customer customer) {
            if (id != customer.CustomerId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                // Verificar Email único excluindo o próprio utilizador
                if (await _context.Customer.AnyAsync(c => c.Email == customer.Email && c.CustomerId != customer.CustomerId) ||
                    await _context.Employee.AnyAsync(e => e.Email == customer.Email)) {
                    ModelState.AddModelError(nameof(Customer.Email), "Email is already in use.");
                }
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details),
                        new {
                            id = customer.CustomerId,
                            SuccessMessage = "Customer updated successfully."
                        }
                    );
                }
                catch (DbUpdateConcurrencyException) {
                    if (!CustomerExists(customer.CustomerId)) {
                        ViewBag.CustomerWasDeleted = true; // Feedback visual se foi apagado entretanto
                    }
                    else {
                        throw;
                    }
                }
                // Se apanhar erro de concorrência mas o registo existir, retorna à View
                if (ViewBag.CustomerWasDeleted != true)
                    return RedirectToAction(nameof(Index));
            }

            ViewData["LevelId"] = new SelectList(_context.Level.OrderBy(l => l.LevelNumber), "LevelId", "LevelNumber", customer.LevelId);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var customer = await _context.Customer
                .Include(c => c.Level)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null) {
                TempData["ErrorMessage"] = "The Customer is no longer available.";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var customer = await _context.Customer.FindAsync(id);

            if (customer != null) {
                try {
                    _context.Customer.Remove(customer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Customer deleted successfully.";
                }
                catch (Exception) {
                    // Padrão EventTypes: Apanhar erro genérico de BD
                    customer = await _context.Customer
                        .AsNoTracking()
                        .Include(c => c.Level)
                        .FirstOrDefaultAsync(c => c.CustomerId == id);

                    // Mensagem genérica pois Customer não tem lista de dependências explícita neste Model,
                    // mas pode estar ligado a outras tabelas (ex: Vendas, Inscrições)
                    ViewBag.Error = "Unable to delete Customer. Ensure they have no active records linked to them.";

                    return View(customer);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id) {
            return _context.Customer.Any(e => e.CustomerId == id);
        }
    }
}