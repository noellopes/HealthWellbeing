using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ClienteServicosController : Controller
    {
        public IActionResult Index()
        {
            var lista = ClienteServicoService.GetAll();
            return View(lista);
        }

        public IActionResult Details(int id)
        {
            var servico = ClienteServicoService.GetById(id);
            if (servico == null)
                return NotFound();

            return View(servico);
        }
    }
}
