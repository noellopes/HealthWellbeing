using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1;
using HealthWellbeing.Utils.Group1.DTOs;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HealthWellbeing.Controllers
{
    public class TreatmentRecordsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly IRecordFilterService<TreatmentRecord> _filterService;

        public TreatmentRecordsController(HealthWellbeingDbContext context, IRecordFilterService<TreatmentRecord> filterService)
        {
            _context = context;
            _filterService = filterService;
        }

        // GET: TreatmentRecords
        public async Task<IActionResult> Index(string searchBy, string searchString, string sortOrder, int page = 1)
        {
            // Controla quantos itens por pagina
            var MAX_ITEMS_PER_PAGE = Constants.MAX_ITEMS_PER_PAGE<TreatmentRecord>();

            // Query Base para otimizar as consultas
            IQueryable<TreatmentRecord> treatments = _context.TreatmentRecord.Include(t => t.Nurse).Include(t => t.Pathology).Include(t => t.TreatmentType).AsNoTracking();

            // Aplica filtragem com os parametros atuais
            treatments = _filterService.ApplyFilter(treatments, searchBy, searchString);
            treatments = _filterService.ApplySorting(treatments, sortOrder);

            // Define as propriadades visiveis do modelo
            DtoSelector BaseSelector = new(
                t => new TreatmentRecordListDTO
                {
                    Id = t.Id,
                    Nurse = t.Nurse.Name,
                    TreatmentType = t.TreatmentType.Name,
                    Pathology = t.Pathology.Name,
                    TreatmentDate = t.TreatmentDate,
                    CompletedDuration = t.CompletedDuration
                },
                ["NurseName", "TreatmentTypeName", "PathologyName", "TreatmentDate", "CompletedDuration"]
            );

            DtoSelector AdminSelector = new(t => new TreatmentRecordListDTO
            {
                Id = t.Id,
                Nurse = t.Nurse.Name,
                TreatmentType = t.TreatmentType.Name,
                Pathology= t.Pathology.Name,
                TreatmentDate = t.TreatmentDate,
                CompletedDuration = t.CompletedDuration,
                Observations = t.Observations ?? "-",
                AdditionalNotes = t.AdditionalNotes ?? "-",
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt
            },
                ["NurseName", "TreatmentTypeName", "PathologyName", "TreatmentDate", "CompletedDuration", "Observations", "AdditionalNotes", "Status", "CreatedAt"]
            );

            DtoSelector selector = BaseSelector;
            IQueryable<TreatmentRecordListDTO> treatmentsProprieties = treatments.Select(selector.Params).AsNoTracking();

            // Popula os dados necessarios a view
            ViewData["Title"] = "Lista de tratamentos";
            ViewBag.ModelType = typeof(TreatmentRecordListDTO);
            ViewBag.Properties = selector.DisplayFields.ToList();
            ViewBag.SearchProperties = _filterService.SearchableProperties;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentPage = page;

            // Aplica paginação e devolve a view com o modelo paginado
            var paginatedList = await PaginatedList<TreatmentRecordListDTO>.CreateAsync(treatmentsProprieties, page, MAX_ITEMS_PER_PAGE);
            return View(paginatedList);
        }

        // GET: TreatmentRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("~/Views/Shared/Group1/NotFound.cshtml");
            }

            var treatmentRecord = await _context.TreatmentRecord
                .Include(t => t.Nurse)
                .Include(t => t.Pathology)
                .Include(t => t.TreatmentType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatmentRecord == null)
            {
                return View("~/Views/Shared/Group1/NotFound.cshtml");
            }
            IReadOnlyList<string> baseProperties = ["Nurse.Name", "TreatmentType.Name", "Pathology.Name", "TreatmentDate", "AdditionalNotes", "Observations", "CompletedDuration", "Status", "CreatedAt"];

            ViewData["Title"] = "Detalhes do tratamento";
            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = baseProperties.ToList();
            return View("~/Views/Shared/Group1/Actions/Details.cshtml", treatmentRecord);
        }

        // GET: TreatmentRecords/Schedule
        //[Authorize(Roles = "Administrator")]
        public IActionResult Schedule()
        {
            ViewData["Title"] = "Marcação de tratamento";
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name");
            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name");
            return View();
        }

        // POST: TreatmentRecords/Schedule
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule([Bind("Id,TreatmentTypeId,PathologyId,TreatmentDate")] TreatmentRecord treatmentRecord)
        {
            ViewData["Title"] = "Marcação de tratamento";

            if (ModelState.IsValid)
            {
                //treatmentRecord.CreatedAt = DateTime.Now;
                treatmentRecord.NurseId = Random.Shared.Next(1, 21);
                _context.Add(treatmentRecord);
                await _context.SaveChangesAsync();
                TempData["Alert"] = AlertItem.CreateAlert("success", "bi bi-check-circle", "O pedido de agendamento de tratamento foi registado com sucesso", true);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name", treatmentRecord.TreatmentTypeId);
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name", treatmentRecord.PathologyId);
            return View(treatmentRecord);
        }

        // GET: TreatmentRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentRecord = await _context.TreatmentRecord.FindAsync(id);
            if (treatmentRecord == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Editar marcação de tratamento";
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name");
            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name");
            return View("Request", treatmentRecord);
        }

        // POST: TreatmentRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdditionalNotes,Status")] TreatmentRecord treatmentRecord)
        {
            if (id != treatmentRecord.Id)
            {
                return NotFound();
            }

            ViewData["Title"] = "Editar marcação de tratamento";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatmentRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentRecordExists(treatmentRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var alert = new AlertItem
                {
                    AlertType = "success",
                    IconClass = "bi bi-check-circle",
                    Message = "Treatment record edited successfully.",
                    Dismissible = true
                };
                TempData["Alert"] = Validator.TryValidateObject(alert, new ValidationContext(alert), new List<ValidationResult>(), true) ? JsonConvert.SerializeObject(alert) : null;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NurseId = new SelectList(_context.Nurse, "Id", "Name", treatmentRecord.NurseId);
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name", treatmentRecord.PathologyId);
            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name", treatmentRecord.TreatmentTypeId);
            return View("Request", treatmentRecord);
        }

        // GET: TreatmentRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentRecord = await _context.TreatmentRecord
                .Include(t => t.Nurse)
                .Include(t => t.Pathology)
                .Include(t => t.TreatmentType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatmentRecord == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Remover tratamento";
            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = new List<string> { "Nurse.Name", "TreatmentType.Name", "Pathology.Name", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status", "CreatedAt" };
            return View("~/Views/Shared/Group1/Actions/Delete.cshtml", treatmentRecord);
        }

        // POST: TreatmentRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentRecord = await _context.TreatmentRecord.FindAsync(id);
            if (treatmentRecord != null)
            {
                // Soft Delete
                _context.TreatmentRecord.Remove(treatmentRecord);

                // Hard Delete
                //await Functions.HardDeleteByIdAsync<TreatmentRecord>(_context, treatmentRecord.Id);
            }

            await _context.SaveChangesAsync();
            var alert = new AlertItem
            {
                AlertType = "success",
                IconClass = "bi bi-check-circle",
                Message = "Treatment record deleted successfully.",
                Dismissible = true
            };
            TempData["Alert"] = Validator.TryValidateObject(alert, new ValidationContext(alert), new List<ValidationResult>(), true) ? JsonConvert.SerializeObject(alert) : null;
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentRecordExists(int id)
        {
            return _context.TreatmentRecord.Any(e => e.Id == id);
        }
    }
}
