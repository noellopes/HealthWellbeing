using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class SpecialitiesController : Controller
    {
        // GET: /Specialities
        public IActionResult Index()
        {
            // Dados MOCK só para testar a página
            var especialidades = new List<Specialities>
            {
                new() { IdEspecialidade = 1, Nome = "Cardiologia", Descricao = "Coração e sistema cardiovascular." },
                new() { IdEspecialidade = 2, Nome = "Dermatologia", Descricao = "Pele, cabelo e unhas." },
                new() { IdEspecialidade = 3, Nome = "Pediatria",   Descricao = "Saúde de crianças e adolescentes." }
            };

            // Envia a lista para a View (Views/Specialities/Index.cshtml)
            return View(especialidades);
        }

        // GET: /Specialities/Details/5  (opcional, para página de detalhe)
        public IActionResult Details(int id)
        {
            var esp = new Specialities
            {
                IdEspecialidade = id,
                Nome = "Exemplo",
                Descricao = "Descrição da especialidade…"
            };

            return View(esp); // View: Views/Specialities/Details.cshtml
        }
    }
}