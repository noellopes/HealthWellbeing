namespace HealthWellbeing.Models
{
    public class CompareWithGoalViewModel
    {
        public Client Client { get; set; }
        public Meta Meta { get; set; }
        public NutritionalNeeds CalculatedNeeds { get; set; }

        // Diferenças (Meta - Calculado)
        public decimal CaloriesDifference { get; set; }
        public decimal ProteinDifference { get; set; }
        public decimal CarbsDifference { get; set; }
        public decimal FatsDifference { get; set; }

        // Percentagens de compliance (Meta / Calculado * 100)
        public decimal CaloriesCompliance { get; set; }
        public decimal ProteinCompliance { get; set; }
        public decimal CarbsCompliance { get; set; }
        public decimal FatsCompliance { get; set; }
    }
}