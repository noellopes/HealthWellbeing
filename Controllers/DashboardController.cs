using HealthWellbeing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.TotalUtentes = _context.Utentes.Count();
        ViewBag.UtentesAtivos = _context.Utentes.Count(u => u.Ativo);
        ViewBag.UtentesInativos = _context.Utentes.Count(u => !u.Ativo);

        ViewBag.TotalTerapeutas = _context.Terapeutas.Count();
        ViewBag.TotalServicos = _context.Servicos.Count();
        ViewBag.TotalAgendamentos = _context.Agendamentos.Count();

        return View();
    }
}
