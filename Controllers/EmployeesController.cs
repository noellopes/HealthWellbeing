using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    [Authorize(Roles = "Gestor")]
    public class EmployeesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeesController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employee.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        private async Task UserShouldBeInRoleAsync(IdentityUser user, string role, bool shouldBeInRole)
        {
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                if (shouldBeInRole)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
            else if (!shouldBeInRole)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeInputModel employee)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Employee.AnyAsync(e => e.Email == employee.Email) || await _context.Customer.AnyAsync(c => c.Email == employee.Email))
                {
                    ModelState.AddModelError(nameof(Employee.Email), "Email is already in use.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(new Employee
                {
                    Name = employee.Name,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber
                });
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByNameAsync(employee.Email);
                if (user == null)
                {
                    user = new IdentityUser(employee.Email) { Email = employee.Email, UserName = employee.Email };
                    var password = "Secret123$"; // TODO: Generate a secure password and send to employee email
                    await _userManager.CreateAsync(user, password);
                }

                await UserShouldBeInRoleAsync(user, "ProductManager", employee.IsProductManager);
                await UserShouldBeInRoleAsync(user, "Administrator", employee.IsAdministrator);

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dbEmployee = await _context.Employee.FindAsync(id);
            if (dbEmployee == null) return NotFound();

            var employee = new EmployeeInputModel
            {
                EmployeeId = id,
                Name = dbEmployee.Name,
                Email = dbEmployee.Email,
                PhoneNumber = dbEmployee.PhoneNumber
            };

            var user = await _userManager.FindByNameAsync(employee.Email);
            if (user != null)
            {
                employee.IsAdministrator = await _userManager.IsInRoleAsync(user, "Administrator");
                employee.IsProductManager = await _userManager.IsInRoleAsync(user, "ProductManager");
            }

            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeInputModel employee, string originalEmail = "")
        {
            if (id != employee.EmployeeId) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Employee.AnyAsync(e => e.Email == employee.Email && e.EmployeeId != employee.EmployeeId) || await _context.Customer.AnyAsync(c => c.Email == employee.Email))
                {
                    ModelState.AddModelError(nameof(Employee.Email), "Email is already in use.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(new Employee
                    {
                        EmployeeId = id,
                        Name = employee.Name,
                        Email = employee.Email,
                        PhoneNumber = employee.PhoneNumber
                    });
                    await _context.SaveChangesAsync();

                    IdentityUser? user = null;
                    if (!string.IsNullOrWhiteSpace(originalEmail))
                    {
                        user = await _userManager.FindByNameAsync(originalEmail);
                    }

                    if (user == null)
                    {
                        user = await _userManager.FindByNameAsync(employee.Email);
                    }

                    if (user != null)
                    {
                        // update identity email if changed
                        if (!string.Equals(user.Email, employee.Email, StringComparison.OrdinalIgnoreCase))
                        {
                            user.Email = employee.Email;
                            user.UserName = employee.Email;
                            await _userManager.UpdateAsync(user);
                        }

                        await UserShouldBeInRoleAsync(user, "ProductManager", employee.IsProductManager);
                        await UserShouldBeInRoleAsync(user, "Administrator", employee.IsAdministrator);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                // Prevent a user from deleting their own account
                if (User?.Identity?.Name != null &&
                    string.Equals(User.Identity.Name, employee.Email, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "You cannot delete your own account.");
                    return View(employee);
                }

                // Attempt to delete associated Identity user (if any)
                var user = await _userManager.FindByNameAsync(employee.Email);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var err in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, err.Description);
                        }
                        // Deletion of the user failed — surface errors to the confirm page
                        return View(employee);
                    } else
                    {
                        _context.Employee.Remove(employee);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.EmployeeId == id);
        }
    }
}
