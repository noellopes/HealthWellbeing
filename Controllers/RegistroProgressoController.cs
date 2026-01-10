using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class RegistroProgressoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private const int PageSize = 10;

        public RegistroProgressoController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: RegistroProgresso
        public async Task<IActionResult> Index(string? searchString, DateTime? dateFrom, DateTime? dateTo, int page = 1)
        {
            var userId = _userManager.GetUserId(User);

            IQueryable<ProgressRecord> query = _context.ProgressRecord
                .Include(p => p.Client)
                .Include(p => p.Nutritionist);

            if (User.IsInRole("Nutricionista"))
            {
                // Nutritionist sees ALL client records
                // (no filter by NutritionistId - can manage all clients)
            }
            else if (User.IsInRole("Cliente"))
            {
                // Client sees only their own records
                var client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == userId);
                if (client == null)
                {
                    return View(new ProgressRecordIndexViewModel 
                    { 
                        Records = new List<ProgressRecord>(), 
                        PageIndex = 1, 
                        TotalPages = 0, 
                        TotalCount = 0, 
                        PageSize = PageSize 
                    });
                }
                query = query.Where(p => p.ClientId == client.ClientId);
            }
            else
            {
                return Forbid();
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(p => p.Client != null && p.Client.Name.Contains(searchString));
            }

            // Apply date filters
            if (dateFrom.HasValue)
            {
                query = query.Where(p => p.RecordDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(p => p.RecordDate <= dateTo.Value);
            }

            // Store filter values for view
            ViewData["CurrentSearch"] = searchString;
            ViewData["DateFrom"] = dateFrom?.ToString("yyyy-MM-dd");
            ViewData["DateTo"] = dateTo?.ToString("yyyy-MM-dd");

            var totalCount = await query.CountAsync();
            var records = await query
                .OrderByDescending(p => p.RecordDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new ProgressRecordIndexViewModel
            {
                Records = records,
                PageIndex = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                TotalCount = totalCount,
                PageSize = PageSize
            };

            return View(viewModel);
        }

        // GET: RegistroProgresso/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.ProgressRecord
                .Include(p => p.Client)
                .Include(p => p.Nutritionist)
                .FirstOrDefaultAsync(m => m.ProgressRecordId == id);

            if (record == null) return NotFound();

            // Authorization check
            if (!await CanAccessRecord(record))
                return Forbid();

            return View(record);
        }

        // GET: RegistroProgresso/Create
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Create()
        {
            ViewData["ClientId"] = new SelectList(await _context.Client.ToListAsync(), "ClientId", "Name");
            return View();
        }

        // POST: RegistroProgresso/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Create([Bind("ClientId,Weight,BodyFatPercentage,Cholesterol,BMI,MuscleMass,RecordDate,Notes")] ProgressRecord record)
        {
            var userId = _userManager.GetUserId(User);
            record.NutritionistId = userId!;
            record.CreatedAt = DateTime.Now;
            record.CreatedBy = userId;

            ModelState.Remove("NutritionistId");
            ModelState.Remove("CreatedBy");

            if (ModelState.IsValid)
            {
                _context.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(await _context.Client.ToListAsync(), "ClientId", "Name", record.ClientId);
            return View(record);
        }

        // GET: RegistroProgresso/Edit/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.ProgressRecord.FindAsync(id);
            if (record == null) return NotFound();

            // Nutritionist can edit any record
            ViewData["ClientId"] = new SelectList(await _context.Client.ToListAsync(), "ClientId", "Name", record.ClientId);
            return View(record);
        }

        // POST: RegistroProgresso/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Edit(int id, [Bind("ProgressRecordId,ClientId,NutritionistId,Weight,BodyFatPercentage,Cholesterol,BMI,MuscleMass,RecordDate,Notes,CreatedAt,CreatedBy")] ProgressRecord record)
        {
            if (id != record.ProgressRecordId) return NotFound();

            ModelState.Remove("NutritionistId");
            ModelState.Remove("CreatedBy");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgressRecordExists(record.ProgressRecordId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(await _context.Client.ToListAsync(), "ClientId", "Name", record.ClientId);
            return View(record);
        }

        // GET: RegistroProgresso/Delete/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.ProgressRecord
                .Include(p => p.Client)
                .Include(p => p.Nutritionist)
                .FirstOrDefaultAsync(m => m.ProgressRecordId == id);

            if (record == null) return NotFound();

            // Nutritionist can delete any record
            return View(record);
        }

        // POST: RegistroProgresso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.ProgressRecord.FindAsync(id);
            if (record != null)
            {
                // Nutritionist can delete any record
                _context.ProgressRecord.Remove(record);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: RegistroProgresso/History/clientId
        public async Task<IActionResult> History(string? id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var userId = _userManager.GetUserId(User);

            // Check access
            if (User.IsInRole("Cliente"))
            {
                var client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == userId);
                if (client == null || client.ClientId != id)
                    return Forbid();
            }

            var records = await _context.ProgressRecord
                .Include(p => p.Client)
                .Where(p => p.ClientId == id)
                .OrderBy(p => p.RecordDate)
                .ToListAsync();

            var client2 = await _context.Client.FindAsync(id);
            var meta = await _context.MetaCorporal
                .Where(m => m.ClientId == id && m.Ativo)
                .OrderByDescending(m => m.CriadoEm)
                .FirstOrDefaultAsync();

            var viewModel = new ProgressHistoryViewModel
            {
                Records = records,
                Meta = meta,
                ClientName = client2?.Name ?? "Cliente",
                ClientId = id
            };

            // Calculate differences if we have at least 2 records
            if (records.Count >= 2)
            {
                var first = records.First();
                var last = records.Last();

                viewModel.FirstRecord = first;
                viewModel.LastRecord = last;

                viewModel.WeightDiff = (last.Weight.HasValue && first.Weight.HasValue) 
                    ? last.Weight - first.Weight : null;
                viewModel.BodyFatDiff = (last.BodyFatPercentage.HasValue && first.BodyFatPercentage.HasValue) 
                    ? last.BodyFatPercentage - first.BodyFatPercentage : null;
                viewModel.CholesterolDiff = (last.Cholesterol.HasValue && first.Cholesterol.HasValue) 
                    ? last.Cholesterol - first.Cholesterol : null;
                viewModel.BMIDiff = (last.BMI.HasValue && first.BMI.HasValue) 
                    ? last.BMI - first.BMI : null;
                viewModel.MuscleMassDiff = (last.MuscleMass.HasValue && first.MuscleMass.HasValue) 
                    ? last.MuscleMass - first.MuscleMass : null;

                // Calculate progress towards goals if meta exists
                if (meta != null && first != null)
                {
                    if (meta.PesoObjetivo.HasValue && first.Weight.HasValue && last.Weight.HasValue)
                    {
                        var totalChange = meta.PesoObjetivo.Value - first.Weight.Value;
                        var currentChange = last.Weight.Value - first.Weight.Value;
                        if (totalChange != 0)
                            viewModel.WeightProgress = (currentChange / totalChange) * 100;
                    }

                    if (meta.GorduraCorporalObjetivo.HasValue && first.BodyFatPercentage.HasValue && last.BodyFatPercentage.HasValue)
                    {
                        var totalChange = meta.GorduraCorporalObjetivo.Value - first.BodyFatPercentage.Value;
                        var currentChange = last.BodyFatPercentage.Value - first.BodyFatPercentage.Value;
                        if (totalChange != 0)
                            viewModel.BodyFatProgress = (currentChange / totalChange) * 100;
                    }

                    if (meta.ColesterolObjetivo.HasValue && first.Cholesterol.HasValue && last.Cholesterol.HasValue)
                    {
                        var totalChange = meta.ColesterolObjetivo.Value - first.Cholesterol.Value;
                        var currentChange = last.Cholesterol.Value - first.Cholesterol.Value;
                        if (totalChange != 0)
                            viewModel.CholesterolProgress = (currentChange / totalChange) * 100;
                    }

                    if (meta.IMCObjetivo.HasValue && first.BMI.HasValue && last.BMI.HasValue)
                    {
                        var totalChange = meta.IMCObjetivo.Value - first.BMI.Value;
                        var currentChange = last.BMI.Value - first.BMI.Value;
                        if (totalChange != 0)
                            viewModel.BMIProgress = (currentChange / totalChange) * 100;
                    }

                    if (meta.MassaMuscularObjetivo.HasValue && first.MuscleMass.HasValue && last.MuscleMass.HasValue)
                    {
                        var totalChange = meta.MassaMuscularObjetivo.Value - first.MuscleMass.Value;
                        var currentChange = last.MuscleMass.Value - first.MuscleMass.Value;
                        if (totalChange != 0)
                            viewModel.MuscleMassProgress = (currentChange / totalChange) * 100;
                    }
                }
            }

            return View(viewModel);
        }

        // GET: RegistroProgresso/Suggestions/clientId
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Suggestions(string? id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var records = await _context.ProgressRecord
                .Include(p => p.Client)
                .Where(p => p.ClientId == id)
                .OrderBy(p => p.RecordDate)
                .ToListAsync();

            var client = await _context.Client.FindAsync(id);
            var meta = await _context.MetaCorporal
                .Where(m => m.ClientId == id && m.Ativo)
                .OrderByDescending(m => m.CriadoEm)
                .FirstOrDefaultAsync();

            ViewData["ClientName"] = client?.Name ?? "Cliente";
            ViewData["ClientId"] = id;

            // Generate suggestions based on MetaCorporal goals
            var suggestions = new List<DietSuggestion>();

            if (records.Count >= 1 && meta != null)
            {
                var current = records.Last(); // Most recent record

                // Weight analysis compared to goal
                if (current.Weight.HasValue && meta.PesoObjetivo.HasValue)
                {
                    var weightDiff = current.Weight.Value - meta.PesoObjetivo.Value;
                    if (Math.Abs(weightDiff) > 5.0m)
                    {
                        if (weightDiff > 0)
                        {
                            suggestions.Add(new DietSuggestion
                            {
                                Category = "Peso",
                                Icon = "⚠️",
                                Priority = "Alta",
                                PriorityClass = "danger",
                                Title = "Acima do peso objetivo",
                                Description = $"Peso atual: {current.Weight:F1} kg. Meta: {meta.PesoObjetivo:F1} kg. Diferença: +{weightDiff:F1} kg.",
                                Recommendation = "Recomenda-se reduzir a ingestão calórica diária em 300-500 kcal. Foque em alimentos integrais, aumente o consumo de vegetais e pratique exercício cardiovascular 3-5x por semana."
                            });
                        }
                        else
                        {
                            suggestions.Add(new DietSuggestion
                            {
                                Category = "Peso",
                                Icon = "⚠️",
                                Priority = "Alta",
                                PriorityClass = "warning",
                                Title = "Abaixo do peso objetivo",
                                Description = $"Peso atual: {current.Weight:F1} kg. Meta: {meta.PesoObjetivo:F1} kg. Diferença: {weightDiff:F1} kg.",
                                Recommendation = "Recomenda-se aumentar a ingestão calórica com alimentos nutritivos e densos em energia. Inclua mais proteínas, carboidratos complexos e gorduras saudáveis nas refeições."
                            });
                        }
                    }
                    else if (Math.Abs(weightDiff) <= 2.0m)
                    {
                        suggestions.Add(new DietSuggestion
                        {
                            Category = "Peso",
                            Icon = "✅",
                            Priority = "Baixa",
                            PriorityClass = "success",
                            Title = "Peso próximo do objetivo",
                            Description = $"Peso atual: {current.Weight:F1} kg. Meta: {meta.PesoObjetivo:F1} kg. Está muito perto!",
                            Recommendation = "Continue com o plano alimentar atual. Faça pequenos ajustes se necessário para atingir o objetivo final."
                        });
                    }
                }

                // Body fat analysis compared to goal
                if (current.BodyFatPercentage.HasValue && meta.GorduraCorporalObjetivo.HasValue)
                {
                    var fatDiff = current.BodyFatPercentage.Value - meta.GorduraCorporalObjetivo.Value;
                    if (fatDiff > 3.0m)
                    {
                        suggestions.Add(new DietSuggestion
                        {
                            Category = "Gordura Corporal",
                            Icon = "⚠️",
                            Priority = "Média",
                            PriorityClass = "warning",
                            Title = "Gordura corporal acima do objetivo",
                            Description = $"Gordura atual: {current.BodyFatPercentage:F1}%. Meta: {meta.GorduraCorporalObjetivo:F1}%. Diferença: +{fatDiff:F1}%.",
                            Recommendation = "Reduza gorduras saturadas e açúcares. Aumente exercícios cardiovasculares e HIIT. Consuma mais proteínas magras e fibras para promover saciedade."
                        });
                    }
                    else if (Math.Abs(fatDiff) <= 2.0m)
                    {
                        suggestions.Add(new DietSuggestion
                        {
                            Category = "Gordura Corporal",
                            Icon = "✅",
                            Priority = "Baixa",
                            PriorityClass = "success",
                            Title = "Gordura corporal próxima do objetivo",
                            Description = $"Gordura atual: {current.BodyFatPercentage:F1}%. Meta: {meta.GorduraCorporalObjetivo:F1}%.",
                            Recommendation = "Excelente progresso! Mantenha a dieta equilibrada e o treino regular."
                        });
                    }
                }

                // Cholesterol analysis compared to goal
                if (current.Cholesterol.HasValue && meta.ColesterolObjetivo.HasValue)
                {
                    var cholDiff = current.Cholesterol.Value - meta.ColesterolObjetivo.Value;
                    if (cholDiff > 20.0m)
                    {
                        suggestions.Add(new DietSuggestion
                        {
                            Category = "Colesterol",
                            Icon = "⚠️",
                            Priority = "Alta",
                            PriorityClass = "danger",
                            Title = "Colesterol acima do objetivo",
                            Description = $"Colesterol atual: {current.Cholesterol:F0} mg/dL. Meta: {meta.ColesterolObjetivo:F0} mg/dL. Diferença: +{cholDiff:F0} mg/dL.",
                            Recommendation = "Reduza gorduras saturadas, carnes vermelhas e alimentos processados. Aumente consumo de fibras solúveis (aveia, leguminosas), peixes ricos em ómega-3 e nozes."
                        });
                    }
                    else if (Math.Abs(cholDiff) <= 10.0m)
                    {
                        suggestions.Add(new DietSuggestion
                        {
                            Category = "Colesterol",
                            Icon = "✅",
                            Priority = "Baixa",
                            PriorityClass = "success",
                            Title = "Colesterol próximo do objetivo",
                            Description = $"Colesterol atual: {current.Cholesterol:F0} mg/dL. Meta: {meta.ColesterolObjetivo:F0} mg/dL.",
                            Recommendation = "Continue com a dieta saudável para manter níveis controlados."
                        });
                    }
                }

                // BMI analysis compared to goal
                if (current.BMI.HasValue && meta.IMCObjetivo.HasValue)
                {
                    var bmiDiff = current.BMI.Value - meta.IMCObjetivo.Value;
                    if (Math.Abs(bmiDiff) > 2.0m)
                    {
                        if (bmiDiff > 0)
                        {
                            suggestions.Add(new DietSuggestion
                            {
                                Category = "IMC",
                                Icon = "⚠️",
                                Priority = "Média",
                                PriorityClass = "warning",
                                Title = "IMC acima do objetivo",
                                Description = $"IMC atual: {current.BMI:F1}. Meta: {meta.IMCObjetivo:F1}.",
                                Recommendation = "Combine défice calórico moderado com exercício regular para reduzir peso de forma saudável (0.5-1kg por semana)."
                            });
                        }
                        else
                        {
                            suggestions.Add(new DietSuggestion
                            {
                                Category = "IMC",
                                Icon = "⚠️",
                                Priority = "Média",
                                PriorityClass = "warning",
                                Title = "IMC abaixo do objetivo",
                                Description = $"IMC atual: {current.BMI:F1}. Meta: {meta.IMCObjetivo:F1}.",
                                Recommendation = "Aumente gradualmente a ingestão calórica com alimentos nutritivos. Foque em ganho de massa muscular com treino de força."
                            });
                        }
                    }
                }

                // Muscle mass analysis compared to goal
                if (current.MuscleMass.HasValue && meta.MassaMuscularObjetivo.HasValue)
                {
                    var muscleDiff = current.MuscleMass.Value - meta.MassaMuscularObjetivo.Value;
                    if (muscleDiff < -3.0m)
                    {
                        suggestions.Add(new DietSuggestion
                        {
                            Category = "Massa Muscular",
                            Icon = "⚠️",
                            Priority = "Alta",
                            PriorityClass = "warning",
                            Title = "Massa muscular abaixo do objetivo",
                            Description = $"Massa muscular atual: {current.MuscleMass:F1} kg. Meta: {meta.MassaMuscularObjetivo:F1} kg. Diferença: {muscleDiff:F1} kg.",
                            Recommendation = "Aumente consumo de proteína para 1.8-2.2g/kg de peso. Combine com treino de resistência 3-4x por semana. Inclua fontes como frango, peixe, ovos, leguminosas e suplementos se necessário."
                        });
                    }
                    else if (Math.Abs(muscleDiff) <= 2.0m)
                    {
                        suggestions.Add(new DietSuggestion
                        {
                            Category = "Massa Muscular",
                            Icon = "✅",
                            Priority = "Baixa",
                            PriorityClass = "success",
                            Title = "Massa muscular próxima do objetivo",
                            Description = $"Massa muscular atual: {current.MuscleMass:F1} kg. Meta: {meta.MassaMuscularObjetivo:F1} kg.",
                            Recommendation = "Mantenha o consumo adequado de proteína e continue com treino de força regular."
                        });
                    }
                }
            }

            // If no goals are set
            if (meta == null)
            {
                suggestions.Add(new DietSuggestion
                {
                    Category = "Configuração",
                    Icon = "ℹ️",
                    Priority = "Média",
                    PriorityClass = "info",
                    Title = "Nenhuma meta definida",
                    Description = "Não existem metas corporais definidas para este cliente.",
                    Recommendation = "Configure as metas corporais do cliente para receber sugestões personalizadas baseadas nos objetivos."
                });
            }

            // If no suggestions were generated, add a default one
            if (!suggestions.Any())
            {
                suggestions.Add(new DietSuggestion
                {
                    Category = "Geral",
                    Icon = "✅",
                    Priority = "Baixa",
                    PriorityClass = "success",
                    Title = "Todos os objetivos estão próximos ou atingidos",
                    Description = "O cliente está dentro ou muito perto de todas as metas definidas.",
                    Recommendation = "Mantenha o plano atual e faça pequenos ajustes conforme necessário para manter os resultados."
                });
            }

            return View(suggestions);
        }

        private async Task<bool> CanAccessRecord(ProgressRecord record)
        {
            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Nutricionista"))
            {
                // Nutritionist can access all records
                return true;
            }
            else if (User.IsInRole("Cliente"))
            {
                var client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == userId);
                return client != null && record.ClientId == client.ClientId;
            }
            return false;
        }

        private bool ProgressRecordExists(int id)
        {
            return _context.ProgressRecord.Any(e => e.ProgressRecordId == id);
        }
    }

    // DTO for diet suggestions
    public class DietSuggestion
    {
        public string Category { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string PriorityClass { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
    }
}
