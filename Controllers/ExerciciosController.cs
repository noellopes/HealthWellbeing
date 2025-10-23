using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthWellbeing.Controllers
{
    public class ExercicioController : Controller
    {
        // Lista de músculos em memória
        private static readonly List<GrupoMuscular> todosMusculos = new List<GrupoMuscular>
        {
            new GrupoMuscular { GrupoMuscularId = 1, GrupoMuscularNome = "Bíceps", Musculo="Braços", LocalizacaoCorporal="Bilateral"},
            new GrupoMuscular { GrupoMuscularId = 2, GrupoMuscularNome = "Tríceps", Musculo="Braços", LocalizacaoCorporal="Bilateral"},
            new GrupoMuscular { GrupoMuscularId = 3, GrupoMuscularNome = "Peitoral", Musculo="Tórax", LocalizacaoCorporal="Bilateral"},
            new GrupoMuscular { GrupoMuscularId = 4, GrupoMuscularNome = "Quadríceps", Musculo="Pernas", LocalizacaoCorporal="Bilateral"}
        };

        private static readonly List<Exercicio> exercicios = new List<Exercicio>
        {
            new Exercicio
            {
                ExercicioId = 1,
                ExercicioNome = "Flexões",
                Descricao = "Exercício para fortalecer peitoral e tríceps",
                Duracao = 10,
                Intencidade = 7,
                CaloriasGastas = 100,
                Instrucoes = "1. Deitar no chão com as mãos à largura dos ombros\n2. Manter o corpo reto\n3. Baixar o corpo até o peito quase tocar no chão\n4. Empurrar de volta à posição inicial",
                EquipamentoNecessario = "Nenhum",
                Repeticoes = 15,
                Series = 3,
                Genero = "Unissexo",
                GrupoMuscular = new List<GrupoMuscular> {
                    todosMusculos[2], // Peitoral
                    todosMusculos[1]  // Tríceps
                }
            },
            new Exercicio
            {
                ExercicioId = 2,
                ExercicioNome = "Agachamentos",
                Descricao = "Exercício para fortalecer as pernas",
                Duracao = 15,
                Intencidade = 6,
                CaloriasGastas = 120,
                Instrucoes = "1. Ficar em pé com os pés à largura dos ombros\n2. Baixar como se fosse sentar numa cadeira\n3. Manter as costas retas\n4. Voltar à posição inicial",
                EquipamentoNecessario = "Nenhum",
                Repeticoes = 20,
                Series = 4,
                Genero = "Unissexo",
                GrupoMuscular = new List<GrupoMuscular> {
                    todosMusculos[3]  // Quadríceps
                }
            }
        };

        public IActionResult Index()
        {
            return View(exercicios);
        }

        public IActionResult Detalhes(int id)
        {
            var exercicio = exercicios.FirstOrDefault(e => e.ExercicioId == id);
            if (exercicio == null)
            {
                TempData["ErrorMessage"] = "Exercício não encontrado.";
                return RedirectToAction("Index");
            }
            return View(exercicio);
        }

        // GET: Create
        [HttpGet]
        public IActionResult Create()
        {
            // Carrega os grupos musculares para a ViewBag
            ViewBag.GruposMusculares = todosMusculos
                .Select(g => new SelectListItem
                {
                    Value = g.GrupoMuscularId.ToString(),
                    Text = g.GrupoMuscularNome
                })
                .ToList();

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Exercicio exercicio, List<int> musculosIds)
        {
            // Validação manual para os músculos
            if (musculosIds == null || !musculosIds.Any())
            {
                ModelState.AddModelError("musculosIds", "Selecione pelo menos um grupo muscular.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Associar músculos selecionados ao exercício
                    exercicio.GrupoMuscular = todosMusculos
                        .Where(g => musculosIds.Contains(g.GrupoMuscularId))
                        .ToList();

                    // Gerar novo ID e adicionar à lista
                    exercicio.ExercicioId = exercicios.Any() ? exercicios.Max(e => e.ExercicioId) + 1 : 1;
                    exercicios.Add(exercicio);

                    TempData["SuccessMessage"] = $"Exercício '{exercicio.ExercicioNome}' criado com sucesso!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Erro ao criar exercício: {ex.Message}";
                }
            }

            // Se chegou aqui, houve erro - recarregar os dados
            ViewBag.GruposMusculares = todosMusculos
                .Select(g => new SelectListItem
                {
                    Value = g.GrupoMuscularId.ToString(),
                    Text = g.GrupoMuscularNome
                })
                .ToList();

            return View(exercicio);
        }
    }
}