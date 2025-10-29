using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class TipoExercicioAntigoController : Controller
    {


        // Simulação de dados
        private static List<TipoExercicio> _tiposExercicios = new List<TipoExercicio>
        {
            new TipoExercicio
            {
                TipoExercicioId = 1,
                CategoriaTipoExercicios = "Cardio",
                DescricaoTipoExercicios = "Exercícios para melhorar o sistema cardiovascular",
                NivelDificuldadeTipoExercicios = "Fácil",
                BeneficioTipoExercicios = "Melhora a resistência e queima calorias",
                GruposMuscularesTrabalhadosTipoExercicios = "Coração, Pernas"
            },
            new TipoExercicio
            {
                TipoExercicioId = 2,
                CategoriaTipoExercicios = "Força",
                DescricaoTipoExercicios = "Exercícios para fortalecimento muscular",
                NivelDificuldadeTipoExercicios = "Intermediário",
                BeneficioTipoExercicios = "Aumenta a massa muscular",
                GruposMuscularesTrabalhadosTipoExercicios = "Peito, Costas, Braços"
            }
        };

        public IActionResult Index()
        {
            return View(_tiposExercicios);
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoExercicio model)
        {
            if (ModelState.IsValid)
            {
                // Gerar Id automaticamente
                model.TipoExercicioId = _tiposExercicios.Count > 0 ? _tiposExercicios.Max(t => t.TipoExercicioId) + 1 : 1;

                _tiposExercicios.Add(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var tipo = _tiposExercicios.FirstOrDefault(t => t.TipoExercicioId == id);
            if (tipo == null)
            {
                return NotFound(); // Retorna página 404 se não encontrar
            }
            return View(tipo);
        }

        // GET
        public IActionResult Delete(int id)
        {
            var tipo = _tiposExercicios.FirstOrDefault(t => t.TipoExercicioId == id);
            if (tipo == null) return NotFound();

            return View(tipo);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tipo = _tiposExercicios.FirstOrDefault(t => t.TipoExercicioId == id);
            if (tipo != null)
            {
                _tiposExercicios.Remove(tipo);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET
        public IActionResult Edit(int id)
        {
            var tipo = _tiposExercicios.FirstOrDefault(t => t.TipoExercicioId == id);
            if (tipo == null) return NotFound();

            return View(tipo);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoExercicio model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tipo = _tiposExercicios.FirstOrDefault(t => t.TipoExercicioId == id);
            if (tipo == null) return NotFound();

            // Atualizar os campos
            tipo.CategoriaTipoExercicios = model.CategoriaTipoExercicios;
            tipo.DescricaoTipoExercicios = model.DescricaoTipoExercicios;
            tipo.NivelDificuldadeTipoExercicios = model.NivelDificuldadeTipoExercicios;
            tipo.BeneficioTipoExercicios = model.BeneficioTipoExercicios;
            tipo.GruposMuscularesTrabalhadosTipoExercicios = model.GruposMuscularesTrabalhadosTipoExercicios;

            return RedirectToAction(nameof(Index));
        }

    }
}
