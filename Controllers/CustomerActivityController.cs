using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Utente")]
    public class CustomerActivityController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public CustomerActivityController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Index (Biblioteca de Atividades)
        public async Task<IActionResult> Index(string searchName, int? searchType, int page = 1)
        {
            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.Include(c => c.Level).FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return View("Error");

            ViewBag.CustomerLevel = customer.Level?.LevelNumber ?? 0;
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;

            // Contadores de atividades realizadas
            var activityCounts = await _context.CustomerActivity
                .Where(ca => ca.CustomerId == customer.CustomerId)
                .GroupBy(ca => ca.ActivityId)
                .Select(g => new { ActivityId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.ActivityId, v => v.Count);

            ViewBag.ActivityCounts = activityCounts;

            // Dropdown de Categorias
            ViewBag.TypesList = new SelectList(_context.ActivityType.OrderBy(t => t.Name), "ActivityTypeId", "Name", searchType);

            var activitiesQuery = _context.Activity
                .Include(a => a.ActivityType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                activitiesQuery = activitiesQuery.Where(a => a.ActivityName.Contains(searchName));

            if (searchType.HasValue)
                activitiesQuery = activitiesQuery.Where(a => a.ActivityTypeId == searchType);

            int totalItems = await activitiesQuery.CountAsync();
            int pageSize = 6;
            var paginationInfo = new ViewModels.PaginationInfo<Activity>(page, totalItems, pageSize);

            paginationInfo.Items = await activitiesQuery
                .OrderBy(a => a.ActivityName)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: History
        public async Task<IActionResult> History()
        {
            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return View("Error");

            var historyLogs = await _context.CustomerActivity
                .Include(ca => ca.Activity)
                    .ThenInclude(a => a.ActivityType)
                .Where(ca => ca.CustomerId == customer.CustomerId)
                .OrderByDescending(ca => ca.CompletionDate)
                .ToListAsync();

            return View(historyLogs);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var activity = await _context.Activity.Include(a => a.ActivityType).FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null) return NotFound();
            return View(activity);
        }

        // POST: PerformActivity (COM VERIFICAÇÃO DE NÍVEL)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformActivity(int activityId)
        {
            var userEmail = User.Identity?.Name;
            // IMPORTANTE: Incluir Level para verificar subida
            var customer = await _context.Customer.Include(c => c.Level).FirstOrDefaultAsync(c => c.Email == userEmail);
            var activity = await _context.Activity.FindAsync(activityId);

            if (customer == null || activity == null) return NotFound();

            // 1. Procurar Evento Ativo, Inscrito e que use esta atividade
            var activeEvent = await _context.Event
                .Include(e => e.EventType)
                .Include(e => e.EventActivities).ThenInclude(ea => ea.Activity)
                .Where(e => e.EventStart <= DateTime.Now && e.EventEnd >= DateTime.Now)
                .Where(e => e.EventActivities.Any(ea => ea.ActivityId == activityId))
                .Where(e => _context.CustomerEvent.Any(ce => ce.EventId == e.EventId
                                                          && ce.CustomerId == customer.CustomerId
                                                          && ce.Status == "Enrolled"))
                .FirstOrDefaultAsync();

            int pointsAwarded = 0;
            string feedbackMsg = "";

            // --- CENÁRIO A: Existe Evento Ativo ---
            if (activeEvent != null)
            {
                // Verifica se já fez esta atividade para este evento específico
                bool alreadyDoneForEvent = await _context.CustomerActivity
                    .AnyAsync(ca => ca.CustomerId == customer.CustomerId
                                 && ca.EventId == activeEvent.EventId
                                 && ca.ActivityId == activityId);

                if (!alreadyDoneForEvent)
                {
                    // 1ª VEZ NO EVENTO: 0 PONTOS (Guardar para o fim)
                    pointsAwarded = 0;

                    var newLog = new CustomerActivity
                    {
                        CustomerId = customer.CustomerId,
                        ActivityId = activityId,
                        CompletionDate = DateTime.Now,
                        PointsEarned = 0,
                        EventId = activeEvent.EventId
                    };
                    _context.CustomerActivity.Add(newLog);
                    await _context.SaveChangesAsync(); // Salvar para contar na verificação abaixo

                    // --- VERIFICAR SE O EVENTO FICOU COMPLETO ---
                    int totalActivitiesNeeded = activeEvent.EventActivities.Count();

                    int totalActivitiesDone = await _context.CustomerActivity
                        .Where(ca => ca.CustomerId == customer.CustomerId && ca.EventId == activeEvent.EventId)
                        .Select(ca => ca.ActivityId)
                        .Distinct()
                        .CountAsync();

                    if (totalActivitiesDone >= totalActivitiesNeeded)
                    {
                        // !!! EVENTO COMPLETADO !!!

                        // 1. Somar pontos BASE de todas as atividades
                        int sumBasePoints = activeEvent.EventActivities.Sum(ea => ea.Activity.ActivityReward);

                        // 2. Aplicar Multiplicador (Decimal)
                        decimal multiplier = activeEvent.EventType.EventTypeMultiplier;
                        int finalEventPoints = (int)(sumBasePoints * multiplier);

                        // 3. Dar prémio ao cliente
                        customer.TotalPoints += finalEventPoints;

                        // 4. Fechar o Evento
                        var enrollment = await _context.CustomerEvent
                            .FirstOrDefaultAsync(ce => ce.EventId == activeEvent.EventId && ce.CustomerId == customer.CustomerId);

                        if (enrollment != null)
                        {
                            enrollment.Status = "Completed";
                            enrollment.PointsEarned = finalEventPoints;
                            enrollment.CompletionDate = DateTime.Now;
                        }

                        feedbackMsg = $"EVENT COMPLETED! Earned {finalEventPoints} points (x{multiplier} Bonus)!";
                    }
                    else
                    {
                        int remaining = totalActivitiesNeeded - totalActivitiesDone;
                        feedbackMsg = $"Logged for '{activeEvent.EventName}'. Complete {remaining} more to finish!";
                    }
                }
                else
                {
                    // Repetição no evento: Dá pontos normais
                    pointsAwarded = activity.ActivityReward;
                    CreateStandardLog(customer.CustomerId, activityId, pointsAwarded);
                    customer.TotalPoints += pointsAwarded;
                    feedbackMsg = $"Activity done! +{pointsAwarded} pts (Standard reward).";
                }
            }
            // --- CENÁRIO B: Sem Evento ---
            else
            {
                pointsAwarded = activity.ActivityReward;
                CreateStandardLog(customer.CustomerId, activityId, pointsAwarded);
                customer.TotalPoints += pointsAwarded;
                feedbackMsg = $"Activity done! +{pointsAwarded} pts.";
            }

            // --- NOVO: VERIFICAR SUBIDA DE NÍVEL ---
            await CheckLevelUp(customer);
            // ---------------------------------------

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = feedbackMsg;

            if (TempData["LevelUpMessage"] != null)
            {
                TempData["SuccessMessage"] += " " + TempData["LevelUpMessage"];
            }

            return RedirectToAction(nameof(Index));
        }

        private void CreateStandardLog(int customerId, int activityId, int points)
        {
            _context.CustomerActivity.Add(new CustomerActivity
            {
                CustomerId = customerId,
                ActivityId = activityId,
                CompletionDate = DateTime.Now,
                PointsEarned = points,
                EventId = null
            });
        }

        // --- MÉTODO AUXILIAR DE LEVEL UP (VERSÃO CORRIGIDA) ---
        private async Task CheckLevelUp(Customer customer)
        {
            // 1. Encontrar o nível correto baseado nos pontos atuais
            // Ordenamos DESCENDENTE para apanhar o nível mais alto que os pontos permitem
            var correctLevel = await _context.Level
                .OrderByDescending(l => l.LevelPointsLimit)
                .FirstOrDefaultAsync(l => customer.TotalPoints >= l.LevelPointsLimit);

            // Se não encontrar (ex: 0 pontos), assume o nível mais baixo
            if (correctLevel == null)
            {
                correctLevel = await _context.Level.OrderBy(l => l.LevelPointsLimit).FirstOrDefaultAsync();
            }

            if (correctLevel == null) return; // Se não houver níveis na BD, sai.

            // 2. Comparação Robusta
            // Compara o ID do nível atual do cliente com o ID do nível que devia ter
            int currentLevelId = customer.Level?.LevelId ?? 0;

            if (correctLevel.LevelId != currentLevelId)
            {
                // 3. Atualizar Dados
                customer.Level = correctLevel;

                // FORÇAR o Entity Framework a marcar este cliente como "Modificado"
                _context.Update(customer);

                // 4. Mostrar mensagem
                TempData["LevelUpMessage"] = $"🎉 LEVEL UP! You reached Level {correctLevel.LevelNumber}!";
            }
        }
    }
}