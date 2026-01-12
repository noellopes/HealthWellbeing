// Models/DailyNutritionSummary.cs
using System;

namespace HealthWellbeing.Models
{
    public class DailyNutritionSummary
    {
        public int PlanoAlimentarId { get; set; } // MUDOU: era PlanId
        public DateTime Date { get; set; }
        public decimal TotalCalories { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalCarbs { get; set; }
        public decimal TotalFats { get; set; }
        public decimal TotalFiber { get; set; }
    }
}