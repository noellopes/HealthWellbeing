using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModels
{
    public class MetaCorporalIndexViewModel
    {
        public List<MetaCorporal> Metas { get; set; } = new List<MetaCorporal>();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
