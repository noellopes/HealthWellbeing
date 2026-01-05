using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.ViewModels;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers {

    [Authorize]
    public class MyBadgesController : Controller {

        private readonly HealthWellbeingDbContext _context;

        public MyBadgesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // ==========================================
        // ACTION: Index (Lista de Badges)
        // ==========================================
        public async Task<IActionResult> Index() {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            // 1. Obter Cliente
            var customer = await _context.Customer
                .AsNoTracking()
                .Select(c => new { c.Email, c.CustomerId })
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) {
                TempData["ErrorMessage"] = "Customer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            // 2. Obter Badges Ganhos (Para pintar a verde/branco)
            var earnedBadgesDict = await _context.CustomerBadge
                .Where(cb => cb.CustomerId == customer.CustomerId)
                .ToDictionaryAsync(cb => cb.BadgeId, cb => cb.DateAwarded);

            // 3. Obter Catálogo de Badges
            var badges = await _context.Badge
                .Include(b => b.BadgeType)
                .OrderBy(b => b.BadgeName)
                .AsNoTracking()
                .ToListAsync();

            // 4. Mapear para ViewModel
            var model = badges.Select(b => new MyBadge {
                Badge = b,
                IsEarned = earnedBadgesDict.ContainsKey(b.BadgeId),
                DateAwarded = earnedBadgesDict.ContainsKey(b.BadgeId) ? earnedBadgesDict[b.BadgeId] : null
            }).ToList();

            return View(model);
        }

        // ==========================================
        // ACTION: Details (Cálculo de Progresso Real)
        // ==========================================
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var userEmail = User.Identity?.Name;

            var customer = await _context.Customer
                .Select(c => new { c.Email, c.CustomerId })
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) {
                TempData["ErrorMessage"] = "Customer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            // 1. Carregar o Badge e as Regras (Requirements)
            var badgeEntity = await _context.Badge
                .Include(b => b.BadgeType)
                // Incluímos os Tipos para mostrar os nomes na View (ex: "Yoga Class")
                .Include(b => b.BadgeRequirements!)
                    .ThenInclude(r => r.EventTypes)
                .Include(b => b.BadgeRequirements!)
                    .ThenInclude(r => r.ActivityTypes)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BadgeId == id);

            if (badgeEntity == null) return NotFound();

            // 2. Verificar se já ganhou
            var earnedInfo = await _context.CustomerBadge
                .Where(cb => cb.CustomerId == customer.CustomerId && cb.BadgeId == id)
                .Select(cb => cb.DateAwarded)
                .FirstOrDefaultAsync();

            bool isEarned = earnedInfo != default(DateTime);

            // 3. LOOP DE CÁLCULO INTELIGENTE
            // Vamos iterar sobre cada requisito e perguntar à BD: "Quantos o user já fez?"
            var progressDict = new Dictionary<int, int>();

            if (badgeEntity.BadgeRequirements != null) {
                foreach (var req in badgeEntity.BadgeRequirements) {

                    // Se o badge já foi ganho, assumimos meta cumprida (100%) para ficar bonito visualmente.
                    // Caso contrário, calculamos o real.
                    if (isEarned) {
                        progressDict[req.BadgeRequirementId] = req.TargetValue;
                    }
                    else {
                        // AQUI ESTÁ A LÓGICA QUE PEDISTE:
                        progressDict[req.BadgeRequirementId] = await CalculateRealProgressAsync(customer.CustomerId, req);
                    }
                }
            }

            var model = new MyBadge {
                Badge = badgeEntity,
                IsEarned = isEarned,
                DateAwarded = isEarned ? earnedInfo : null,
                RequirementsProgress = progressDict
            };

            return View(model);
        }

        // ==========================================
        // O CÉREBRO DA LÓGICA (PRIVATE HELPER)
        // ==========================================
        private async Task<int> CalculateRealProgressAsync(int customerId, BadgeRequirement req) {

            // O Switch decide qual tabela consultar com base no Enum RequirementType
            switch (req.RequirementType) {

                // CASO 1: Participar em QUALQUER Evento
                // Query: Tabela EventCustomer -> Count simples pelo CustomerId
                case RequirementType.ParticipateAnyEvent:
                    return await _context.EventCustomer
                        .CountAsync(ec => ec.CustomerId == customerId);

                // CASO 2: Participar num Tipo de Evento ESPECÍFICO (Linked via EventType)
                // Query: Tabela EventCustomer -> JOIN Event -> Filtrar por EventTypeId do Requisito
                case RequirementType.ParticipateSpecificEventType:
                    if (req.EventTypeId == null) return 0; // Salvaguarda

                    return await _context.EventCustomer
                        .Include(ec => ec.Event) // Precisamos de ir à tabela Event saber o tipo
                        .CountAsync(ec => ec.CustomerId == customerId
                                       && ec.Event.EventTypeId == req.EventTypeId);

                // CASO 3: Completar QUALQUER Atividade
                // Query: Tabela ActivityCustomer -> Count simples
                case RequirementType.CompleteAnyActivity:
                    return await _context.ActivityCustomer
                        .CountAsync(ac => ac.CustomerId == customerId);

                // CASO 4: Completar Tipo de Atividade ESPECÍFICA (Linked via ActivityType)
                // Query: Tabela ActivityCustomer -> JOIN Activity -> Filtrar por ActivityTypeId
                case RequirementType.CompleteSpecificActivityType:
                    if (req.ActivityTypeId == null) return 0; // Salvaguarda

                    return await _context.ActivityCustomer
                        .Include(ac => ac.Activity) // Precisamos de ir à tabela Activity saber o tipo
                        .CountAsync(ac => ac.CustomerId == customerId
                                       && ac.Activity.ActivityTypeId == req.ActivityTypeId);

                default:
                    return 0;
            }
        }
    }
}