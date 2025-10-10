using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthWellbeing.Controllers
{
    public class ExercicioController : Controller
    {
        public IActionResult VerExercicios()
        {
            // Para testar dados exemplo
            var exercicios = new List<Exercicio>
            {
                new Exercicio 
                { 
                    ExercicioId = 1,
                    ExercicioNome = "Flexões",
                    Descricao = "Exercício para peitoral e tríceps",
                    Duracao = 10,
                    Intencidade = 7,
                    CaloriasGastas = 100
                },
                new Exercicio 
                { 
                    ExercicioId = 2,
                    ExercicioNome = "Agachamentos",
                    Descricao = "Exercício para pernas e glúteos",
                    Duracao = 15,
                    Intencidade = 5,
                    CaloriasGastas = 150
                }
            };
            
            return View(exercicios);
        }
    

        [HttpGet]
        public IActionResult CriarExercicio()
        {
            // Carrega grupos musculares para o dropdown (dados exemplo)
            ViewBag.GruposMusculares = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Bíceps (Braços) - Bilateral" },
                new SelectListItem { Value = "2", Text = "Tríceps (Braços) - Bilateral" },
                new SelectListItem { Value = "3", Text = "Peitoral (Tórax) - Bilateral" },
                new SelectListItem { Value = "4", Text = "Quadríceps (Pernas) - Bilateral" }
            };

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarExercicio(Exercicio exercicio)
        {
            if (ModelState.IsValid)
            {
                // Guatdar na base de dados

                TempData["SuccessMessage"] = $"Exercício '{exercicio.ExercicioNome}' criado com sucesso!";
                return RedirectToAction("ExerciciosView");
            }

            // Se houver erros, recarrega os grupos musculares
            ViewBag.GruposMusculares = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Bíceps (Braços) - Bilateral" },
                new SelectListItem { Value = "2", Text = "Tríceps (Braços) - Bilateral" },
                new SelectListItem { Value = "3", Text = "Peitoral (Tórax) - Bilateral" },
                new SelectListItem { Value = "4", Text = "Quadríceps (Pernas) - Bilateral" }
            };

            return View(exercicio);
        }
    }
}
