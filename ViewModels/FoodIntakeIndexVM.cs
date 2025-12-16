using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthWellbeing.ViewModels
{
    public class FoodIntakeIndexVM
    {
        // ----- Filtros do topo -----
        public DateTime SelectedDate { get; set; }

        public int SelectedClientId { get; set; }

        public string? SelectedClientName { get; set; }

        // ----- Combos / Selects -----
        public List<SelectListItem> Clients { get; set; } = new();

        public List<SelectListItem> AvailableFoods { get; set; } = new();

        // ----- Cards apresentados -----
        public List<FoodIntakeCardVM> Items { get; set; } = new();
    }
}
