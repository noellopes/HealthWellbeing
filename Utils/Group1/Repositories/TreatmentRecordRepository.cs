using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.DTOs;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthWellbeing.Utils.Group1.Repositories
{
    public interface ITreatmentRecordRepository
    {
        IReadOnlyList<string> GetSearchableProperties();

        Task<Group1DataResponseObject<TreatmentRecord, TreatmentRecordListDTO>> GetPagedTreatmentRecordsAsync(
            ClaimsPrincipal user,
            string searchBy,
            string searchString,
            string sortOrder,
            int page
        );
    }

    public class TreatmentRecordRepository : ITreatmentRecordRepository
    {

        private readonly HealthWellbeingDbContext _context;
        private readonly IRecordFilterService<TreatmentRecord> _filterService;
        private readonly int MAX_ITEMS_PER_PAGE = Constants.MAX_ITEMS_PER_PAGE<TreatmentRecord>();

        public TreatmentRecordRepository(HealthWellbeingDbContext context, IRecordFilterService<TreatmentRecord> filterService)
        {
            _context = context;
            _filterService = filterService;
        }

        public IReadOnlyList<string> GetSearchableProperties() => _filterService.SearchableProperties;

        public async Task<Group1DataResponseObject<TreatmentRecord, TreatmentRecordListDTO>> GetPagedTreatmentRecordsAsync(ClaimsPrincipal user, string searchBy, string searchString, string sortOrder, int page)
        {
            // Query base para otimizar consultas
            IQueryable<TreatmentRecord> baseQuery = _context.TreatmentRecord.Where(t => t.TreatmentDate >= DateTime.Today && t.Status != TreatmentStatus.Canceled && t.Status != TreatmentStatus.Completed).AsNoTracking();

            // Define as propriadades visiveis do modelo
            DtoSelector<TreatmentRecord, TreatmentRecordListDTO> selector;

            selector = new(t => new TreatmentRecordListDTO
            {
                Id = t.Id,
                Nurse = t.Nurse.Name,
                TreatmentType = t.TreatmentType.Name,
                Pathology = t.Pathology.Name,
                TreatmentDate = t.TreatmentDate,
                CompletedDuration = t.CompletedDuration
            }, [
                nameof(TreatmentRecordListDTO.Nurse),
                nameof(TreatmentRecordListDTO.TreatmentType),
                nameof(TreatmentRecordListDTO.Pathology),
                nameof(TreatmentRecordListDTO.TreatmentDate),
                nameof(TreatmentRecordListDTO.CompletedDuration)
            ]);

            if (user.IsInRole("Nurse"))
            {
                // Filtra os proximos tratamentos agendados atribuídos para o(a) enfermeiro(a)
                //var nurseId = GetNurseIdFromUser(user);
                var nurseId = Random.Shared.Next(1, 21);

                baseQuery = _context.TreatmentRecord
                    .Where(t => t.NurseId == nurseId)
                    .Include(t => t.Nurse)
                    .Include(t => t.Pathology)
                    .Include(t => t.TreatmentType)
                    .AsNoTracking();
            }
            else if (new[] { "Administrator", "TreatmentOfficeManager" }.Any(r => user.IsInRole(r)))
            {
                // Admins
                baseQuery = _context.TreatmentRecord
                    .Include(t => t.Nurse)
                    .Include(t => t.Pathology)
                    .Include(t => t.TreatmentType)
                    .AsNoTracking();

                selector = new(t => new TreatmentRecordListDTO
                {
                    Id = t.Id,
                    Nurse = t.Nurse.Name,
                    TreatmentType = t.TreatmentType.Name,
                    Pathology = t.Pathology.Name,
                    TreatmentDate = t.TreatmentDate,
                    CompletedDuration = t.CompletedDuration,
                    Observations = t.Observations ?? "-",
                    AdditionalNotes = t.AdditionalNotes ?? "-",
                    Status = Functions.GetEnumDisplayName(t.Status),
                    CreatedAt = t.CreatedAt
                }, [
                   nameof(TreatmentRecordListDTO.Nurse),
                    nameof(TreatmentRecordListDTO.TreatmentType),
                    nameof(TreatmentRecordListDTO.Pathology),
                    nameof(TreatmentRecordListDTO.TreatmentDate),
                    nameof(TreatmentRecordListDTO.CompletedDuration),
                    nameof(TreatmentRecordListDTO.Observations),
                    nameof(TreatmentRecordListDTO.AdditionalNotes),
                    nameof(TreatmentRecordListDTO.Status),
                    nameof(TreatmentRecordListDTO.CreatedAt)
               ]);
            }
            else
            {
                // Utilizadores (Utentes)
                baseQuery = _context.TreatmentRecord
                    .Include(t => t.Nurse)
                    .Include(t => t.Pathology)
                    .Include(t => t.TreatmentType)
                    .AsNoTracking();
            }

            // Aplica filtragem com os parametros atuais
            baseQuery = _filterService.ApplyFilter(baseQuery, searchBy, searchString);
            baseQuery = _filterService.ApplySorting(baseQuery, sortOrder);

            var projected = baseQuery.Select(selector.Params).AsNoTracking();
            var paginated = await PaginatedList<TreatmentRecordListDTO>.CreateAsync(projected, page, MAX_ITEMS_PER_PAGE);

            return new Group1DataResponseObject<TreatmentRecord, TreatmentRecordListDTO>(paginated, selector);
        }
    }
}
