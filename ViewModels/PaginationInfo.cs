namespace HealthWellbeing.ViewModels
{
    public class PaginationInfo<T>
    {
        public const int NUMBER_PAGES_SHOW_BEFORE_AFTER = 5;

        public PaginationInfo(int currentPage, int totalItems, int itemsPerPage = 10)
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;

            // TotalPages nunca pode ser < 1
            int totalPages = TotalPages;

            // Se não existirem itens, forçar 1 página
            if (totalPages == 0)
            {
                CurrentPage = 1;
                return;
            }

            // Garante que a página atual está no intervalo válido
            CurrentPage = Math.Clamp(currentPage, 1, totalPages);
        }

        public IEnumerable<T>? Items { get; set; } = null;

        public int TotalItems { get; private set; }

        public int ItemsPerPage { get; private set; }

        public int CurrentPage { get; private set; }

        public int TotalPages =>
            Math.Max(1, (int)Math.Ceiling((double)TotalItems / ItemsPerPage));

        public int FirstPageShow =>
            Math.Max(1, CurrentPage - NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public int LastPageShow =>
            Math.Min(TotalPages, CurrentPage + NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public int ItemsToSkip =>
            ItemsPerPage * (CurrentPage - 1);
    }
}
