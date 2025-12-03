using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.DTOs;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Security.Claims;

namespace HealthWellbeing.Utils.Group1.Repositories
{
    public interface ITreatmentRecordRepository
    {
        IReadOnlyList<string> GetSearchableProperties();

        bool TreatmentRecordExists(Guid id);

        Task<Group1DataResponseObjectSingle<TreatmentRecord, TreatmentRecordListDTO>?> GetSingleTreatmentRecordAsync(ClaimsPrincipal user, Guid id);

        Task<Group1DataResponseObject<TreatmentRecord, TreatmentRecordListDTO>> GetPagedTreatmentRecordsAsync(
            ClaimsPrincipal user,
            string searchBy,
            string searchString,
            string sortOrder,
            int page
        );

        Task<bool> RemoveAsync(Guid id);
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

        public bool TreatmentRecordExists(Guid id)
        {
            return _context.TreatmentRecord.Any(e => e.Id == id);
        }

        public async Task<Group1DataResponseObjectSingle<TreatmentRecord, TreatmentRecordListDTO>?> GetSingleTreatmentRecordAsync(ClaimsPrincipal user, Guid id)
        {
            if (TreatmentRecordExists(id))
            {
                IQueryable<TreatmentRecord> baseQuery = _context.TreatmentRecord.Include(t => t.Nurse).Include(t => t.Pathology).Include(t => t.TreatmentType).AsNoTracking();
                DtoSelector<TreatmentRecord, TreatmentRecordListDTO> selector;

                selector = new(t => new TreatmentRecordListDTO
                {
                    Id = t.Id,
                    Nurse = t.Nurse.Name,
                    TreatmentType = t.TreatmentType.Name,
                    Pathology = t.Pathology.Name,
                    TreatmentDate = t.TreatmentDate.ToShortDateString(),
                    CompletedDuration = t.CompletedDuration
                }, [
                    nameof(TreatmentRecordListDTO.Nurse),
                    nameof(TreatmentRecordListDTO.TreatmentType),
                    nameof(TreatmentRecordListDTO.Pathology),
                    nameof(TreatmentRecordListDTO.TreatmentDate),
                    nameof(TreatmentRecordListDTO.CompletedDuration)
                ]);

                switch (user)
                {
                    case var currentUser when currentUser.IsInRole("Nurse"):
                        // Filtra os proximos tratamentos agendados atribuídos para o(a) enfermeiro(a)
                        //var nurseId = GetNurseIdFromUser(user);
                        var nurseId = Random.Shared.Next(1, 21);
                        baseQuery = baseQuery.Where(t => t.NurseId == nurseId);
                        selector = new(t => new TreatmentRecordListDTO
                        {
                            Id = t.Id,
                            Nurse = t.Nurse.Name,
                            TreatmentType = t.TreatmentType.Name,
                            Pathology = t.Pathology.Name,
                            TreatmentDate = t.TreatmentDate.ToShortDateString(),
                            AdditionalNotes = t.AdditionalNotes,
                            EstimatedDuration = t.EstimatedDuration,
                            Status = Functions.GetEnumDisplayName(t.Status),
                        }, [
                            nameof(TreatmentRecordListDTO.Nurse),
                            nameof(TreatmentRecordListDTO.TreatmentType),
                            nameof(TreatmentRecordListDTO.Pathology),
                            nameof(TreatmentRecordListDTO.TreatmentDate),
                            nameof(TreatmentRecordListDTO.AdditionalNotes),
                            nameof(TreatmentRecordListDTO.EstimatedDuration),
                            nameof(TreatmentRecordListDTO.Status),
                            ]);
                        break;
                    case var currentUser when currentUser.IsInRole("Administrator") || currentUser.IsInRole("TreatmentOfficeManager"):
                        //baseQuery = baseQuery.FirstOrDefaultAsync(m => m.Id == id);
                        selector = new(t => new TreatmentRecordListDTO
                        {
                            Id = t.Id,
                            Nurse = t.Nurse.Name,
                            TreatmentType = t.TreatmentType.Name,
                            Pathology = t.Pathology.Name,
                            TreatmentDate = t.TreatmentDate.ToShortDateString(),
                            AdditionalNotes = t.AdditionalNotes,
                            EstimatedDuration = t.EstimatedDuration,
                            Observations = t.Observations,
                            Status = Functions.GetEnumDisplayName(t.Status),
                            CompletedDuration = t.CompletedDuration,
                            CreatedAt = t.CreatedAt
                        }, [
                           nameof(TreatmentRecordListDTO.Nurse),
                           nameof(TreatmentRecordListDTO.TreatmentType),
                           nameof(TreatmentRecordListDTO.Pathology),
                           nameof(TreatmentRecordListDTO.TreatmentDate),
                           nameof(TreatmentRecordListDTO.AdditionalNotes),
                           nameof(TreatmentRecordListDTO.EstimatedDuration),
                           nameof(TreatmentRecordListDTO.Observations),
                           nameof(TreatmentRecordListDTO.Status),
                           nameof(TreatmentRecordListDTO.CompletedDuration),
                           nameof(TreatmentRecordListDTO.CreatedAt)
                           ]);
                        break;
                    default:
                        // Utilizadores (Utentes)
                        //var patientId = Random.Shared.Next(1, 21);
                        //baseQuery = baseQuery.FirstOrDefaultAsync(m => m.Id == id);
                        break;
                }

                var projected = await baseQuery.Select(selector.Params).FirstOrDefaultAsync(m => m.Id == id);

                return new Group1DataResponseObjectSingle<TreatmentRecord, TreatmentRecordListDTO>(projected, selector);
            }

            return null;
        }

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
                TreatmentDate = t.TreatmentDate.ToShortDateString(),
                CompletedDuration = t.CompletedDuration
            }, [
                nameof(TreatmentRecordListDTO.Nurse),
                nameof(TreatmentRecordListDTO.TreatmentType),
                nameof(TreatmentRecordListDTO.Pathology),
                nameof(TreatmentRecordListDTO.TreatmentDate),
                nameof(TreatmentRecordListDTO.CompletedDuration)
            ]);

            switch (user)
            {
                case var currentUser when currentUser.IsInRole("Nurse"):
                    // Filtra os proximos tratamentos agendados atribuídos para o(a) enfermeiro(a)
                    //var nurseId = GetNurseIdFromUser(user);
                    var nurseId = Random.Shared.Next(1, 21);
                    baseQuery = baseQuery
                        .Where(t => t.NurseId == nurseId)
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
                        TreatmentDate = t.TreatmentDate.ToShortDateString(),
                        AdditionalNotes = t.AdditionalNotes,
                        EstimatedDuration = t.EstimatedDuration,
                        Status = Functions.GetEnumDisplayName(t.Status),
                        CreatedAt = t.CreatedAt
                    }, [
                       nameof(TreatmentRecordListDTO.Nurse),
                       nameof(TreatmentRecordListDTO.TreatmentType),
                       nameof(TreatmentRecordListDTO.Pathology),
                       nameof(TreatmentRecordListDTO.TreatmentDate),
                       nameof(TreatmentRecordListDTO.AdditionalNotes),
                       nameof(TreatmentRecordListDTO.EstimatedDuration),
                       nameof(TreatmentRecordListDTO.Status),
                       nameof(TreatmentRecordListDTO.CreatedAt)
                       ]);
                    break;
                case var currentUser when currentUser.IsInRole("Administrator") || currentUser.IsInRole("TreatmentOfficeManager"):
                    baseQuery = baseQuery
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
                        TreatmentDate = t.TreatmentDate.ToShortDateString(),
                        AdditionalNotes = t.AdditionalNotes,
                        EstimatedDuration = t.EstimatedDuration,
                        Observations = t.Observations,
                        Status = Functions.GetEnumDisplayName(t.Status),
                        CompletedDuration = t.CompletedDuration,
                        CreatedAt = t.CreatedAt
                    }, [
                       nameof(TreatmentRecordListDTO.Nurse),
                       nameof(TreatmentRecordListDTO.TreatmentType),
                       nameof(TreatmentRecordListDTO.Pathology),
                       nameof(TreatmentRecordListDTO.TreatmentDate),
                       nameof(TreatmentRecordListDTO.AdditionalNotes),
                       nameof(TreatmentRecordListDTO.EstimatedDuration),
                       nameof(TreatmentRecordListDTO.Observations),
                       nameof(TreatmentRecordListDTO.Status),
                       nameof(TreatmentRecordListDTO.CompletedDuration),
                       nameof(TreatmentRecordListDTO.CreatedAt)
                       ]);
                    break;
                default:
                    // Utilizadores (Utentes)
                    baseQuery = _context.TreatmentRecord
                        .Include(t => t.Nurse)
                        .Include(t => t.Pathology)
                        .Include(t => t.TreatmentType)
                        .AsNoTracking();
                    break;
            }

            // Aplica filtragem com os parametros atuais
            baseQuery = _filterService.ApplyFilter(baseQuery, searchBy, searchString);
            baseQuery = _filterService.ApplySorting(baseQuery, sortOrder);

            var projected = baseQuery.Select(selector.Params).AsNoTracking();
            var paginated = await PaginatedList<TreatmentRecordListDTO>.CreateAsync(projected, page, MAX_ITEMS_PER_PAGE);

            return new Group1DataResponseObject<TreatmentRecord, TreatmentRecordListDTO>(paginated, selector);
        }

        // Soft Delete
        async Task<bool> ITreatmentRecordRepository.RemoveAsync(Guid id)
        {
            if (TreatmentRecordExists(id))
            {
                await _context.TreatmentRecord.Where(c => c.Id == id).ExecuteDeleteAsync();
                return true;
            }

            return false;
        }
    }
}
