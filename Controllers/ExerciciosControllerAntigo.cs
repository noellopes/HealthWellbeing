using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System;

namespace HealthWellbeing.Controllers
{
    public class ExercicioController : Controller
    {
        private static readonly List<GrupoMuscular> todosMusculos = new()
        {
            new GrupoMuscular { GrupoMuscularId = 1, GrupoMuscularNome = "Bíceps", LocalizacaoCorporal="Superior" },
            new GrupoMuscular { GrupoMuscularId = 2, GrupoMuscularNome = "Tríceps", LocalizacaoCorporal="Superior" },
            new GrupoMuscular { GrupoMuscularId = 3, GrupoMuscularNome = "Peitoral", LocalizacaoCorporal="Superior" },
            new GrupoMuscular { GrupoMuscularId = 4, GrupoMuscularNome = "Quadríceps", LocalizacaoCorporal="Inferior" }
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
                return NotFound();

            return View(exercicio);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CarregarGruposMusculares();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Exercicio model, List<int> musculosIds)
        {
            if (musculosIds == null || !musculosIds.Any())
                ModelState.AddModelError("musculosIds", "Selecione pelo menos um grupo muscular.");

            if (!ModelState.IsValid)
            {
                CarregarGruposMusculares();
                return View(model);
            }

            model.ExercicioId = exercicios.Any() ? exercicios.Max(e => e.ExercicioId) + 1 : 1;
            model.GrupoMuscular = todosMusculos.Where(g => musculosIds.Contains(g.GrupoMuscularId)).ToList();
            exercicios.Add(model);

            TempData["SuccessMessage"] = "Exercício criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var exercicio = exercicios.FirstOrDefault(e => e.ExercicioId == id);
            if (exercicio == null) return NotFound();

            CarregarGruposMusculares(exercicio.GrupoMuscular.Select(m => m.GrupoMuscularId).ToList());
            return View(exercicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Exercicio model, List<int> musculosIds)
        {
            var exercicio = exercicios.FirstOrDefault(e => e.ExercicioId == id);
            if (exercicio == null) return NotFound();

            if (!ModelState.IsValid)
            {
                CarregarGruposMusculares(musculosIds);
                return View(model);
            }

            exercicio.ExercicioNome = model.ExercicioNome;
            exercicio.Descricao = model.Descricao;
            exercicio.Duracao = model.Duracao;
            exercicio.Intencidade = model.Intencidade;
            exercicio.CaloriasGastas = model.CaloriasGastas;
            exercicio.Instrucoes = model.Instrucoes;
            exercicio.EquipamentoNecessario = model.EquipamentoNecessario;
            exercicio.Genero = model.Genero;
            exercicio.Series = model.Series;
            exercicio.Repeticoes = model.Repeticoes;
            exercicio.GrupoMuscular = todosMusculos.Where(g => musculosIds.Contains(g.GrupoMuscularId)).ToList();

            TempData["SuccessMessage"] = "Exercício atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var exercicio = exercicios.FirstOrDefault(e => e.ExercicioId == id);
            if (exercicio == null) return NotFound();
            return View(exercicio);
        }

        [HttpPost]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var exercicio = exercicios.FirstOrDefault(e => e.ExercicioId == id);
            if (exercicio != null)
            {
                exercicios.Remove(exercicio);
                TempData["SuccessMessage"] = "Exercício removido com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        private void CarregarGruposMusculares(List<int>? preSelecionados = null)
        {
            ViewBag.GruposMusculares = todosMusculos.Select(g =>
                new SelectListItem
                {
                    Value = g.GrupoMuscularId.ToString(),
                    Text = g.GrupoMuscularNome,
                    Selected = preSelecionados != null && preSelecionados.Contains(g.GrupoMuscularId)
                }).ToList();
        }
    }
}
