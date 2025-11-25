namespace HealthWellbeing.Utils.Group1.Models
{
    public class PaginationProps
    {
        public required int CurrentPage { get; set; }
        public required bool PrevDisabled { get; set; }
        public required bool NextDisabled { get; set; }
        public required int TotalPages { get; set; }
        public int FirstPageShow => Math.Max(1, CurrentPage - Constants.NUMBER_PAGES_SHOW_BEFORE_AFTER);
        public int LastPageShow => Math.Min(TotalPages, CurrentPage + Constants.NUMBER_PAGES_SHOW_BEFORE_AFTER);
    }
}
