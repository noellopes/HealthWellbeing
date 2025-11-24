using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Models;
using HealthWellbeing.Utils.Group1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class TreatmentRecordsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly ITreatmentRecordRepository _repository;
        private IReadOnlyList<string> searchableProperties;

        public TreatmentRecordsController(HealthWellbeingDbContext context, ITreatmentRecordRepository repository)
        {
            _context = context;
            _repository = repository;
            searchableProperties = _repository.GetSearchableProperties();
        }

        // GET: TreatmentRecords
        [Authorize(Roles = "Patient, Nurse, TreatmentOfficeManager, Administrator")]
        public async Task<IActionResult> Index(string searchBy, string searchString, string sortOrder, int page = 1)
        {
            var treatments = await _repository.GetPagedTreatmentRecordsAsync(User, searchBy, searchString, sortOrder, page);

            // Popula os dados necessarios a view
            ViewData["Title"] = "Lista de tratamentos";
            ViewBag.ModelType = treatments.DTOType;
            ViewBag.Properties = treatments.Selector.DisplayFields.ToList();
            ViewBag.SearchProperties = searchableProperties.ToList();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentPage = page;

            // Devolve a view com o modelo de paginação e DTO
            return View(treatments.Data);
        }

        // GET: TreatmentRecords/Details/<GUID>
        [Authorize(Roles = "Patient, Nurse, TreatmentOfficeManager, Administrator")]
        public async Task<IActionResult> Details(Guid id)
        {
            var treatmentRecord = await _repository.GetSingleTreatmentRecordAsync(User, id);
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
        [Authorize(Roles = "Patient")]
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
        [Authorize(Roles = "Patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule([Bind("Id,TreatmentTypeId,PathologyId,TreatmentDate")] TreatmentRecord treatmentRecord)
        {
            ViewData["Title"] = "Marcação de tratamento";

            if (ModelState.IsValid)
            {
                treatmentRecord.NurseId = Random.Shared.Next(1, 21);
                treatmentRecord.EstimatedDuration = Random.Shared.Next(1, 121);
                _context.Add(treatmentRecord);
                await _context.SaveChangesAsync();
                TempData["Alert"] = AlertItem.CreateAlert("success", "bi bi-check-circle", "O pedido de agendamento de tratamento foi registado com sucesso", true);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name", treatmentRecord.TreatmentTypeId);
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name", treatmentRecord.PathologyId);
            return View(treatmentRecord);
        }

        // GET: TreatmentRecords/Edit/<GUID>
        [Authorize(Roles = "Patient, Nurse, TreatmentOfficeManager, Administrator")]
        public async Task<IActionResult> Edit(Guid id)
        {
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

        // POST: TreatmentRecords/Edit/<GUID>
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient, Nurse, TreatmentOfficeManager")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AdditionalNotes,Status")] TreatmentRecord treatmentRecord)
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
                    if (!_repository.TreatmentRecordExists(treatmentRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Alert"] = AlertItem.CreateAlert("success", "bi bi-check-circle", "Os dados do tratamento foram atualizados com sucesso", true);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NurseId = new SelectList(_context.Nurse, "Id", "Name", treatmentRecord.NurseId);
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name", treatmentRecord.PathologyId);
            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name", treatmentRecord.TreatmentTypeId);
            return View("Request", treatmentRecord);
        }

        // GET: TreatmentRecords/Delete/5
        [Authorize(Roles = "TreatmentOfficeManager, Administrator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var treatmentRecord = await _repository.GetSingleTreatmentRecordAsync(User, id);
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
        [Authorize(Roles = "TreatmentOfficeManager, Administrator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var treatmentRecord = await _repository.GetSingleTreatmentRecordAsync(User, id);
            if (treatmentRecord == null)
            {
                return NotFound();
            }
            else
            {
                // Soft Delete
                _context.TreatmentRecord.Remove(treatmentRecord);

                // Hard Delete
                //await Functions.HardDeleteByIdAsync<TreatmentRecord>(_context, treatmentRecord.Id);

                await _context.SaveChangesAsync();

                TempData["Alert"] = AlertItem.CreateAlert("success", "bi bi-check-circle", "O registo do tratamento foi removido com sucesso", true);
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
