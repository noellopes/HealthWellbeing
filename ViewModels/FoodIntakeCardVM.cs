using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthWellbeing.ViewModels
{
    public class FoodIntakeCardVM
    {
        public int FoodId { get; set; }

        // Id da linha na tabela FoodIntake (ou equivalente)
        public int? FoodIntakeId { get; set; }

        public string FoodName { get; set; } = "";

        public string? PortionName { get; set; }

        public bool IsConsumed { get; set; }

        // Bootstrap icon (ex: "bi-egg-fried")
        public string? IconClass { get; set; }

        // Opcional
        public string? Note { get; set; }
    }
}
