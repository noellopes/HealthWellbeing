using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Interfaces;

namespace HealthWellbeing.Utils.Group1.Services
{
    public class TreatmentRecordFilterService : IRecordFilterService<TreatmentRecord>
    {
        private readonly Dictionary<string, Func<IQueryable<TreatmentRecord>, string, IQueryable<TreatmentRecord>>> _filterMap = new(StringComparer.OrdinalIgnoreCase) {
            { "Nurse", (query, val) => query.Where(t => t.Nurse.Name.Contains(val)) },
            { "TreatmentType", (query, val) => query.Where(t => t.TreatmentType.Name.Contains(val)) },
            { "Pathology", (query, val) => query.Where(t => t.Pathology.Name.Contains(val)) },
            { "Status", (query, val) => query.Where(t => t.Status.ToString().Contains(val)) }
        };

        public IReadOnlyList<string> SearchableProperties => _filterMap.Keys.ToList();

        public IQueryable<TreatmentRecord> ApplyFilter(IQueryable<TreatmentRecord> query, string searchBy, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchBy) || string.IsNullOrWhiteSpace(searchString)) return query;

            if (_filterMap.TryGetValue(searchBy, out var filterFunc))
            {
                return filterFunc(query, searchString);
            }

            return query;
        }

        public IQueryable<TreatmentRecord> ApplySorting(IQueryable<TreatmentRecord> query, string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "TreatmentDate_desc";
            }

            bool descending = sortOrder.EndsWith("_desc", StringComparison.OrdinalIgnoreCase);
            var sortProperty = descending ? sortOrder[..^5] : sortOrder;

            return sortProperty switch
            {
                "Nurse" => descending ? query.OrderByDescending(t => t.Nurse.Name) : query.OrderBy(t => t.Nurse.Name),
                "TreatmentType" => descending ? query.OrderByDescending(t => t.TreatmentType.Name) : query.OrderBy(t => t.TreatmentType.Name),
                "Pathology" => descending ? query.OrderByDescending(t => t.Pathology.Name) : query.OrderBy(t => t.Pathology.Name),
                "TreatmentDate" => descending ? query.OrderByDescending(t => t.TreatmentDate) : query.OrderBy(t => t.TreatmentDate),
                "CompletedDuration" => descending ? query.OrderByDescending(t => t.CompletedDuration) : query.OrderBy(t => t.CompletedDuration),
                "Status" => descending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                "CreatedAt" => descending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                _ => descending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
            };
        }
    }
}
