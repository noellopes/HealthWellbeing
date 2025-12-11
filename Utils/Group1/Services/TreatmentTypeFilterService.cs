using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Interfaces;

namespace HealthWellbeing.Utils.Group1.Services
{
    public class TreatmentTypeFilterService : IRecordFilterService<TreatmentType>
    {
        private readonly Dictionary<string, Func<IQueryable<TreatmentType>, string, IQueryable<TreatmentType>>> _filterMap = new(StringComparer.OrdinalIgnoreCase) {
            { "Name", (query, val) => query.Where(t => t.Name.Contains(val)) },
            { "EstimatedDuration", (query, val) => query.Where(t => t.EstimatedDuration.ToString().Contains(val)) },
        };

        public IReadOnlyList<string> SearchableProperties => _filterMap.Keys.ToList();

        public IQueryable<TreatmentType> ApplyFilter(IQueryable<TreatmentType> query, string searchBy, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchBy) || string.IsNullOrWhiteSpace(searchString)) return query;

            if (_filterMap.TryGetValue(searchBy, out var filterFunc))
            {
                return filterFunc(query, searchString);
            }

            return query;
        }

        public IQueryable<TreatmentType> ApplySorting(IQueryable<TreatmentType> query, string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "Name";
            }

            bool descending = sortOrder.EndsWith("_desc", StringComparison.OrdinalIgnoreCase);
            var sortProperty = descending ? sortOrder[..^5] : sortOrder;

            return sortProperty switch
            {
                "Name" => descending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                "Description" => descending ? query.OrderByDescending(t => t.Description) : query.OrderBy(t => t.Description),
                "EstimatedDuration" => descending ? query.OrderByDescending(t => t.EstimatedDuration) : query.OrderBy(t => t.EstimatedDuration),
                _ => descending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
            };
        }
    }
}
