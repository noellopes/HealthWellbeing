using System;

namespace HealthWellbeing.Models
{
    public class NutritionalNeeds
    {
        public string ClientId { get; set; } // string agora
        public string ClientName { get; set; }
        public decimal DailyCalories { get; set; }
        public decimal TMB { get; set; }
        public decimal TDEE { get; set; }

        public decimal ProteinGrams { get; set; }
        public decimal CarbohydratesGrams { get; set; }
        public decimal FatsGrams { get; set; }

        public decimal ProteinPercentage { get; set; }
        public decimal CarbsPercentage { get; set; }
        public decimal FatsPercentage { get; set; }

        public decimal FiberGrams { get; set; }
        public decimal SodiumMg { get; set; }
        public decimal CalciumMg { get; set; }
        public decimal IronMg { get; set; }

        public DateTime CalculatedDate { get; set; }
    }
}