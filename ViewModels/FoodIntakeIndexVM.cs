using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthWellbeing.ViewModels
{
    public class FoodIntakeIndexVM
    {
        // ===== GOAL =====
        public int DailyCaloriesGoal { get; set; }
        public int DailyProteinGoal { get; set; }
        public int DailyFatGoal { get; set; }
        public int DailyHydratesGoal { get; set; }

        // ===== CONSUMED =====
        public double CaloriesConsumed { get; set; }
        public double ProteinConsumed { get; set; }
        public double FatConsumed { get; set; }
        public double HydratesConsumed { get; set; }

        // ===== DERIVED =====
        public double CaloriesRemaining => Math.Max(0, DailyCaloriesGoal - CaloriesConsumed);
        public double ProteinRemaining => Math.Max(0, DailyProteinGoal - ProteinConsumed);
        public double FatRemaining => Math.Max(0, DailyFatGoal - FatConsumed);
        public double HydratesRemaining => Math.Max(0, DailyHydratesGoal - HydratesConsumed);

        // Percentagens (para barras)
        public double CaloriesPct => DailyCaloriesGoal == 0 ? 0 : CaloriesConsumed / DailyCaloriesGoal * 100;
        public double ProteinPct => DailyProteinGoal == 0 ? 0 : ProteinConsumed / DailyProteinGoal * 100;
        public double FatPct => DailyFatGoal == 0 ? 0 : FatConsumed / DailyFatGoal * 100;
        public double HydratesPct => DailyHydratesGoal == 0 ? 0 : HydratesConsumed / DailyHydratesGoal * 100;



        public DateTime SelectedDate { get; set; }

        public int SelectedClientId { get; set; }

        public string? SelectedClientName { get; set; }

        public List<SelectListItem> Clients { get; set; } = new();
        public List<SelectListItem> Portions { get; set; } = new();
        public List<SelectListItem> AvailableFoods { get; set; } = new();


        public List<FoodIntakeCardVM> Items { get; set; } = new();
    }
}
