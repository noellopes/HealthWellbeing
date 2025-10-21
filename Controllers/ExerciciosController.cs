using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthWellbeing.Controllers
{
    public class ExercicioController : Controller
    {
        // Lista de músculos em memória (simulando base de dados)
        private static readonly List<GruposMusculares> todosMusculos = new List<GruposMusculares>
        {
            new GruposMusculares { MusculoId = 1, MusculoNome = "Bíceps", GrupoMuscularPrimario="Braços", LadoMusculo="Bilateral", TamanhoMusculo=10 },
            new GruposMusculares { MusculoId = 2, MusculoNome = "Tríceps", GrupoMuscularPrimario="Braços", LadoMusculo="Bilateral", TamanhoMusculo=12 },
            new GruposMusculares { MusculoId = 3, MusculoNome = "Peitoral", GrupoMuscularPrimario="Tórax", LadoMusculo="Bilateral", TamanhoMusculo=20 },
            new GruposMusculares { MusculoId = 4, MusculoNome = "Quadríceps", GrupoMuscularPrimario="Pernas", LadoMusculo="Bilateral", TamanhoMusculo=25 }
        };

        // Lista de exercícios em memória para testar
        private static readonly List<Exercicio> exercicios = new List<Exercicio>();

        // View para listar exercícios
        public IActionResult Index()
        {
            return View(exercicios);
        }

        // GET: Create
        [HttpGet]
        public IActionResult Create()
        {
            // Passa músculos para a View
            ViewBag.GruposMusculares = todosMusculos
                .Select(g => new SelectListItem { Value = g.MusculoId.ToString(), Text = g.MusculoNome })
                .ToList();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Exercicio exercicio, [FromForm] List<int> musculosIds)
        {
            if (ModelState.IsValid)
            {
                // Associar músculos selecionados ao exercício
                if (musculosIds != null && musculosIds.Any())
                {
                    exercicio.GruposMusculares = todosMusculos
                        .Where(g => musculosIds.Contains(g.MusculoId))
                        .ToList();
                }
                else
                {
                    exercicio.GruposMusculares = new List<GruposMusculares>();
                }

                // Simula salvar em memória
                exercicio.ExercicioId = exercicios.Count + 1;
                exercicios.Add(exercicio);

                TempData["SuccessMessage"] = $"Exercício '{exercicio.ExercicioNome}' criado com sucesso!";
                return RedirectToAction("Index");
            }

            // Se houver erro, recarrega músculos
            ViewBag.GruposMusculares = todosMusculos
                .Select(g => new SelectListItem { Value = g.MusculoId.ToString(), Text = g.MusculoNome })
                .ToList();

            return View(exercicio);
        }
    }
}
