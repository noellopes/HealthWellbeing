

namespace HealthWellbeing.ViewModels
{
    public class Paginacao<T>
    {
        public const int NUMBER_PAGES_SHOW_BEFORE_AFTER = 3;

        public Paginacao(int currentPage, int totalItems, int itemsPerPage = 10)
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            CurrentPage = Math.Clamp(currentPage, 1, TotalPages);
        }

        public IEnumerable<T>
            ? Items
        { get; set; }

        public int TotalItems { get; }
        public int ItemsPerPage { get; }
        public int CurrentPage { get; }
        public int TotalPages => Math.Max(1,
        (int)Math.Ceiling((double)TotalItems / ItemsPerPage));

        public int FirstPageShow => Math.Max(1, CurrentPage - NUMBER_PAGES_SHOW_BEFORE_AFTER);
        public int LastPageShow => Math.Min(TotalPages, CurrentPage + NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public int ItemsToSkip => ItemsPerPage * (CurrentPage - 1);
    }
}