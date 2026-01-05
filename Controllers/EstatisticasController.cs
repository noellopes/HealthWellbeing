using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeing.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class EstatisticasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public EstatisticasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchProfissional = "")
        {
            var query = _context.UtenteGrupo7.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                query = query.Where(u => u.Nome.Contains(searchNome));
            }

            if (!string.IsNullOrEmpty(searchProfissional))
            {
                query = query.Where(u => u.ProfissionalSaudeId.Contains(searchProfissional));
            }

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchProfissional = searchProfissional;

            int total = await query.CountAsync();
            var pagination = new PaginationInfo<UtenteGrupo7>(page, total);

            if (total > 0)
            {
                pagination.Items = await query
                    .Select(u => new UtenteGrupo7
                    {
                        UtenteGrupo7Id = u.UtenteGrupo7Id,
                        Nome = u.Nome,
                        ProfissionalSaudeId = u.ProfissionalSaudeId,
                        UserId = u.UserId
                    })
                    .OrderBy(u => u.Nome)
                    .Skip(pagination.ItemsToSkip)
                    .Take(pagination.ItemsPerPage)
                    .ToListAsync();
            }
            else
            {
                pagination.Items = new List<UtenteGrupo7>();
            }

            return View(pagination);
        }

        public async Task<IActionResult> MeuProgresso()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge();

            var utente = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == userId);

            if (utente == null)
            {
                TempData["ErrorMessage"] = "Ficha de utente não encontrada.";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Utente", new { id = utente.UtenteGrupo7Id });
        }

        public async Task<IActionResult> Utente(int? id)
        {
            if (id == null) return NotFound();

            var utente = await _context.UtenteGrupo7.FindAsync(id);
            if (utente == null) return NotFound();

            if (!User.IsInRole("Administrador") && !User.IsInRole("ProfissionalSaude"))
            {
                var userIdLogado = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (utente.UserId != userIdLogado) return Forbid();
            }

            var planos = await _context.PlanoExercicios
                .Include(p => p.PlanoExercicioExercicios)
                    .ThenInclude(pe => pe.Exercicio)
                .Where(p => p.UtenteGrupo7Id == id)
                .OrderBy(p => p.PlanoExerciciosId)
                .ToListAsync();

            var todosExerciciosList = planos
                .SelectMany(p => p.PlanoExercicioExercicios ?? new List<PlanoExercicioExercicio>())
                .ToList();

            int totalExercicios = todosExerciciosList.Count;
            int exerciciosConcluidos = todosExerciciosList.Count(e => e.Concluido);
            int exerciciosPendentes = totalExercicios - exerciciosConcluidos;

            int planosFeitos = 0;
            foreach (var p in planos)
            {
                if (p.PlanoExercicioExercicios != null && p.PlanoExercicioExercicios.Any())
                {
                    if (p.PlanoExercicioExercicios.All(e => e.Concluido))
                        planosFeitos++;
                }
            }

            string exercicioFavorito = "Nenhum";
            if (todosExerciciosList.Any())
            {
                var grupoFav = todosExerciciosList
                    .GroupBy(e => e.Exercicio != null ? e.Exercicio.ExercicioNome : "Desconhecido")
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                if (grupoFav != null)
                    exercicioFavorito = grupoFav.Key;
            }

            var listaBarras = new List<DadoBarra>();
            int contador = 1;
            double volumeTotalVida = 0;

            foreach (var plano in planos)
            {
                double somaPlano = 0;

                if (plano.PlanoExercicioExercicios != null)
                {
                    somaPlano = plano.PlanoExercicioExercicios
                        .Where(e => e.PesoUsado.HasValue)
                        .Sum(e => e.PesoUsado.Value);
                }

                volumeTotalVida += somaPlano;

                listaBarras.Add(new DadoBarra
                {
                    Label = $"P{contador}",
                    ValorReal = somaPlano
                });
                contador++;
            }

            double maiorCarga = listaBarras.Any() ? listaBarras.Max(b => b.ValorReal) : 1;
            if (maiorCarga == 0) maiorCarga = 1;

            foreach (var barra in listaBarras)
            {
                barra.AlturaPercentagem = (int)((barra.ValorReal / maiorCarga) * 100);
            }

            var viewModel = new UtenteEstatisticasViewModel
            {
                NomeUtente = utente.Nome,
                TotalExercicios = totalExercicios,
                QtdConcluidos = exerciciosConcluidos,
                QtdNaoConcluidos = exerciciosPendentes,
                EvolucaoCarga = listaBarras,
                ExercicioFavorito = exercicioFavorito,
                VolumeTotalAcumulado = volumeTotalVida,
                TotalPlanosAtribuidos = planos.Count,
                TotalPlanos100Porcento = planosFeitos
            };

            return View(viewModel);
        }
    }
}