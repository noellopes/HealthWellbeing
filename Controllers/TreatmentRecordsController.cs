using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Models;

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
        public async Task<IActionResult> Index(string sortOrder, string searchBy, string searchString, int pageNumber = 1)
        {
            // Controla quantos itens por pagina
            var MAX_ITEMS_PER_PAGE = 1;

            // Define as propriadades visiveis do modelo
            IReadOnlyList<string> baseProperties = ["Nurse.Name", "TreatmentType.Name", "Pathology.Name", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status", "CreatedAt"];

            // Query Base para otimizar as consultas
            IQueryable<TreatmentRecord> treatments = _context.TreatmentRecord.Include(t => t.Nurse).Include(t => t.Pathology).Include(t => t.TreatmentType).AsNoTracking();

            // Aplica filtragem com os parametros atuais
            treatments = _filterService.ApplyFilter(treatments, searchBy, searchString);
            treatments = _filterService.ApplySorting(treatments, sortOrder);

            // Popula os dados necessarios a view
            ViewData["Title"] = "Lista de tratamentos";
            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = baseProperties.ToList();
            ViewBag.SearchProperties = _filterService.SearchableProperties;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentPage = pageNumber;

            // Aplica paginação e devolve a view com o modelo paginado
            var paginatedList = await PaginatedList<TreatmentRecord>.CreateAsync(treatments, pageNumber, MAX_ITEMS_PER_PAGE);
            return View("~/Views/Shared/Group1/Actions/Index.cshtml", paginatedList);
        }

        // GET: TreatmentRecords/Details/5
        public async Task<IActionResult> Details(int? id)
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

            ViewData["Title"] = "Detalhes do tratamento";
            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = new List<string> { "Nurse.Name", "TreatmentType.Name", "Pathology.Name", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status", "CreatedAt" };
            return View("~/Views/Shared/Group1/Actions/Details.cshtml", treatmentRecord);
        }

        // GET: TreatmentRecords/Create
        //[Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Marcação de tratamento";
            ViewBag.NurseId = new SelectList(_context.Nurse, "Id", "Name");
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name");
            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name");
            return View("CreateOrEdit");
        }

        // POST: TreatmentRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NurseId,TreatmentId,PathologyId,TreatmentDate,DurationMinutes,Remarks,Result,Status")] TreatmentRecord treatmentRecord)
        {
            ViewData["Title"] = "Marcação de tratamento";
            treatmentRecord.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(treatmentRecord);
                await _context.SaveChangesAsync();
                TempData["Alert"] = AlertItem.CreateAlert("success", "bi bi-check-circle", "Treatment record created successfully.", true);
                return RedirectToAction(nameof(Index));
            }

            ViewData["NurseId"] = new SelectList(_context.Nurse, "Id", "Name", treatmentRecord.NurseId);
            ViewData["PathologyId"] = new SelectList(_context.Pathology, "Id", "Name", treatmentRecord.PathologyId);
            ViewData["TreatmentId"] = new SelectList(_context.TreatmentType, "Id", "Name", treatmentRecord.TreatmentId);
            return View("CreateOrEdit", treatmentRecord);
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
            ViewBag.NurseId = new SelectList(_context.Nurse, "Id", "Name");
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name");
            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name");
            return View("CreateOrEdit", treatmentRecord);
        }

        // POST: TreatmentRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NurseId,TreatmentId,PathologyId,TreatmentDate,DurationMinutes,Remarks,Result,Status,CreatedAt")] TreatmentRecord treatmentRecord)
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

            ViewData["NurseId"] = new SelectList(_context.Nurse, "Id", "Name", treatmentRecord.NurseId);
            ViewData["PathologyId"] = new SelectList(_context.Pathology, "Id", "Name", treatmentRecord.PathologyId);
            ViewData["TreatmentId"] = new SelectList(_context.TreatmentType, "Id", "Name", treatmentRecord.TreatmentId);
            return View("CreateOrEdit", treatmentRecord);
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
