using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.ViewModels
{
    public class PaginationInfo<T> where T : class
    {
        public const int NUMBER_PAGES_SHOW_BEFORE_AFTER = 5;

        public IEnumerable<T>? Items { get; set; } = null;
        public int TotalItems { get; private set; }
        public int ItemsPerPage { get; private set; }
        public int CurrentPage { get; private set; }

        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalItems / ItemsPerPage));

        public int ItemsToSkip => ItemsPerPage * (CurrentPage - 1);

        public int FirstPageShow => Math.Max(1, CurrentPage - NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public int LastPageShow => Math.Min(TotalPages, CurrentPage + NUMBER_PAGES_SHOW_BEFORE_AFTER);

        public PaginationInfo(int currentPage, int totalItems, int itemsPerPage = 5)
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            CurrentPage = Math.Clamp(currentPage, 1, TotalPages);
        }
    }
}