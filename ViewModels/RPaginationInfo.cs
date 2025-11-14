namespace HealthWellbeing.ViewModels
{
    public class RPaginationInfo<T>
    {
        public const int NUMBER_PAGES_SHOW_BEFORE_AFTER = 5;

        public RPaginationInfo(int currentPage, int totalItems, int itemsPerPage = 10)
        {
            //Construtor
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            CurrentPage = Math.Clamp(currentPage, 1, TotalPages);
        }

        public IEnumerable<T>? Items { get; set; } = null;

        public int TotalItems { get; private set; }

        public int ItemsPerPage { get; private set; }

        public int CurrentPage { get; private set; }

        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalItems / ItemsPerPage));

        public int FirstPageShow => Math.Max(1, CurrentPage - NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public int LastPageShow => Math.Min(TotalPages, CurrentPage + NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public int ItemsToSkip => ItemsPerPage * (CurrentPage - 1);
    }
}
