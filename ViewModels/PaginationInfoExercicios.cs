namespace HealthWellbeing.ViewModels
{
    public class PaginationInfoExercicios<T>
    {
        public const int NUMBER_PAGES_SHOW_BEFORE_AFTER = 5;

        public PaginationInfoExercicios(int currentPage, int totalItems, int itemsPerPage = 10)
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;

            if (currentPage < 1)
            {
                CurrentPage = 1;
            }
            else if (currentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
            else
            {
                CurrentPage = currentPage;
            }
        }

        public IEnumerable<T>? Items { get; set; } = null;

        public int TotalItems { get; private set; }

        public int ItemsPerPage { get; private set; }

        public int CurrentPage { get; private set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

        public int FirstPageShow => Math.Max(1, CurrentPage - NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public int LastPageShow => Math.Min(TotalPages, CurrentPage + NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public int ItemsToSkip => ItemsPerPage * (CurrentPage - 1);
    }
}