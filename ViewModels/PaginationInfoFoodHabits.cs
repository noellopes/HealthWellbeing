using System;
using System.Collections.Generic;

namespace HealthWellbeing.ViewModels
{
    public class PaginationInfoFoodHabits<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int CurrentPage { get; set; }

        public int ItemsPerPage { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

        public int FirstPageShow => Math.Max(1, CurrentPage - 2);

        public int LastPageShow => Math.Min(TotalPages, CurrentPage + 2);

        public PaginationInfoFoodHabits() { }

        public PaginationInfoFoodHabits(IEnumerable<T> items, int totalItems, int currentPage, int itemsPerPage)
        {
            Items = items;
            TotalItems = totalItems;
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
        }
    }
}