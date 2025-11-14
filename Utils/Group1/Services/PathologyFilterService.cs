using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Interfaces;

namespace HealthWellbeing.Utils.Group1.Services
{
    public class PathologyFilterService : IRecordFilterService<Pathology>
    {
        private readonly Dictionary<string, Func<IQueryable<Pathology>, string, IQueryable<Pathology>>> _filterMap = new(StringComparer.OrdinalIgnoreCase) {
            { "Name", (query, val) => query.Where(t => t.Name.Contains(val)) },
            { "Severity", (query, val) => query.Where(t => t.Severity.Contains(val)) },
        };

        public IReadOnlyList<string> SearchableProperties => _filterMap.Keys.ToList();

        public IQueryable<Pathology> ApplyFilter(IQueryable<Pathology> query, string searchBy, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchBy) || string.IsNullOrWhiteSpace(searchString)) return query;

            if (_filterMap.TryGetValue(searchBy, out var filterFunc))
            {
                return filterFunc(query, searchString);
            }

            return query;
        }

        public IQueryable<Pathology> ApplySorting(IQueryable<Pathology> query, string sortOrder)
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
                "Severity" => descending ? query.OrderByDescending(t => t.Severity) : query.OrderBy(t => t.Severity),
                _ => descending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
            };
        }
    }
}
