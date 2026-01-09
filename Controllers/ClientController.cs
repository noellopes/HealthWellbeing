using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

public class ClientController : Controller
{
    private readonly HealthWellbeingDbContext _context;

    public ClientController(HealthWellbeingDbContext context)
    {
        _context = context;
    }

    // Index: exibe a lista de clientes com filtros e paginação
    public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchPhone = "", string searchEmail = "", string searchMember = "")
    {
        var clientsQuery = _context.Client
            .Include(c => c.Membership)
            .AsQueryable();

        // Filtros de pesquisa
        if (!string.IsNullOrEmpty(searchName))
            clientsQuery = clientsQuery.Where(c => c.Name.Contains(searchName));

        if (!string.IsNullOrEmpty(searchPhone))
            clientsQuery = clientsQuery.Where(c => c.Phone.Contains(searchPhone));

        if (!string.IsNullOrEmpty(searchEmail))
            clientsQuery = clientsQuery.Where(c => c.Email.Contains(searchEmail));

        if (!string.IsNullOrEmpty(searchMember))
            clientsQuery = searchMember == "Yes"
                ? clientsQuery.Where(c => c.Membership != null)
                : clientsQuery.Where(c => c.Membership == null);

        // Passando parâmetros de pesquisa para a View
        ViewBag.SearchName = searchName;
        ViewBag.SearchPhone = searchPhone;
        ViewBag.SearchEmail = searchEmail;
        ViewBag.SearchMember = searchMember;

        // Número total de clientes
        int numberClients = await clientsQuery.CountAsync();

        
        var clientsInfo = new PaginationInfo<Client>(
            page,               // Página atual
            numberClients,      // Total de clientes
            5                   // Itens por página
        );

        // Busca dos clientes para a página atual
        clientsInfo.Items = await clientsQuery
            .OrderBy(c => c.Name)
            .Skip((page - 1) * 5) // Páginação simplificada
            .Take(5)
            .ToListAsync();

        return View(clientsInfo);
    }

    // Create: exibe o formulário para criar um novo cliente
    public IActionResult Create()
    {
        return View();
    }

    // Create (POST): processa a criação de um novo cliente
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Phone,Email,MembershipId")] Client client)
    {
        if (ModelState.IsValid)
        {
            _context.Add(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(client);
    }

    // Edit: exibe o formulário para editar um cliente
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await _context.Client.FindAsync(id);
        if (client == null)
        {
            return NotFound();
        }
        return View(client);
    }

    // Edit (POST): processa a atualização de um cliente existente
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ClientId,Name,Phone,Email,MembershipId")] Client client)
    {
        if (id != client.ClientId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(client);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(client.ClientId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(client);
    }

    // Delete: exibe o formulário para confirmar a exclusão de um cliente
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await _context.Client
            .Include(c => c.Membership)
            .FirstOrDefaultAsync(m => m.ClientId == id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    // Delete (POST): processa a exclusão de um cliente
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = await _context.Client.FindAsync(id);
        _context.Client.Remove(client);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ClientExists(int id)
    {
        return _context.Client.Any(e => e.ClientId == id);
    }
}
