using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace HealthWellbeing.Controllers
{
    public class FoodIntakeController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodIntakeController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: FoodIntake
        public async Task<IActionResult> Index(int? clientId, DateTime? selectedDate)
        {
            var date = (selectedDate ?? DateTime.Today).Date;

            // 1) Verificar e resetar os dados se a data for diferente do dia atual
            if (selectedDate.HasValue && selectedDate.Value.Date != DateTime.Today.Date)
            {
                await ResetFoodIntakeForDate(clientId, selectedDate); // Chama o método de reset
            }

            // 2) Clientes que têm planos
            var clientsWithPlans = await _context.Plan
                .Include(p => p.Client)
                .Select(p => p.Client)
                .Distinct()
                .OrderBy(c => c.Name)
                .ToListAsync();

            // 3) SelectList para o dropdown
            ViewBag.ClientList = new SelectList(clientsWithPlans, "ClientId", "Name", clientId);
            ViewBag.ClientId = clientId;

            // 4) Query base dos FoodIntake
            var query = _context.FoodIntake
                .Include(fi => fi.Food)
                .Include(fi => fi.Plan)
                .Include(fi => fi.Portion)
                .Where(fi => fi.Date.Date == date);

            if (clientId.HasValue && clientId.Value > 0)
            {
                query = query.Where(fi => fi.Plan.ClientId == clientId.Value);
            }

            var items = await query
                .OrderBy(fi => fi.ScheduledTime)
                .ToListAsync();

            // 5) Passar os dados para o View
            ViewBag.SelectedDate = date;
            ViewBag.ClientId = clientId;

            // Nome do cliente para mostrar na página (opcional)
            if (clientId.HasValue && clientId.Value > 0)
            {
                var client = await _context.Client.FindAsync(clientId.Value);
                ViewBag.ClientName = client != null ? client.Name : "Cliente não encontrado";
            }
            else
            {
                ViewBag.ClientName = "All the Clients";
            }

            // Estatísticas
            var totalItems = items.Count;
            var consumedCount = items.Count(fi => fi.Eaten);
            var notConsumedCount = totalItems - consumedCount;
            var consumedPercentage = totalItems > 0
                ? Math.Round(consumedCount * 100.0 / totalItems, 1)
                : 0.0;
            var notConsumedPercentage = 100 - consumedPercentage;

            ViewBag.TotalItems = totalItems;
            ViewBag.ConsumedCount = consumedCount;
            ViewBag.NotConsumedCount = notConsumedCount;
            ViewBag.ConsumedPercentage = consumedPercentage;
            ViewBag.NotConsumedPercentage = notConsumedPercentage;

            return View(items);


        }


        [HttpPost]
        public async Task<IActionResult> ToggleEaten(int id, int? clientId, DateTime? selectedDate)
        {
            var foodIntake = await _context.FoodIntake.FindAsync(id);
            if (foodIntake == null)
            {
                return NotFound();
            }

            // alternar true/false
            foodIntake.Eaten = !foodIntake.Eaten;

            _context.Update(foodIntake);
            await _context.SaveChangesAsync();

            // voltar ao mesmo contexto
            return RedirectToAction(nameof(Index), new
            {
                clientId = clientId,
                selectedDate = selectedDate ?? foodIntake.Date
            });
        }

        // Método para resetar a ingestão alimentar
        public async Task<IActionResult> ResetFoodIntakeForDate(int? clientId, DateTime? selectedDate)
        {
            var date = (selectedDate ?? DateTime.Today).Date;

            // Verifica se existe um plano alimentar associado ao cliente e à data
            var planForDate = await _context.Plan
                .Where(p => p.ClientId == clientId && p.StartingDate <= date && p.EndingDate >= date)
                .FirstOrDefaultAsync();

            if (planForDate != null)
            {
                // Encontrar todos os registros de ingestão de alimentos para o cliente e a data selecionada
                var foodIntakesToReset = await _context.FoodIntake
                    .Where(fi => fi.PlanId == planForDate.PlanId && fi.Date.Date == date)
                    .ToListAsync();

                if (foodIntakesToReset.Any())
                {
                    // Remover todos os registros encontrados
                    _context.FoodIntake.RemoveRange(foodIntakesToReset);
                    await _context.SaveChangesAsync();
                }
            }

            // Redireciona para a visualização do plano alimentar com a data atualizada
            return RedirectToAction(nameof(Index), new { clientId = clientId, selectedDate = date });
        }


    }
}