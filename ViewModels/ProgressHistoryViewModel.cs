using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModels
{
    public class ProgressHistoryViewModel
    {
        public List<ProgressRecord> Records { get; set; } = new List<ProgressRecord>();
        public MetaCorporal? Meta { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        
        // Comparison data
        public ProgressRecord? FirstRecord { get; set; }
        public ProgressRecord? LastRecord { get; set; }
        
        // Differences from first to last
        public decimal? WeightDiff { get; set; }
        public decimal? BodyFatDiff { get; set; }
        public decimal? CholesterolDiff { get; set; }
        public decimal? BMIDiff { get; set; }
        public decimal? MuscleMassDiff { get; set; }
        
        // Progress towards goals (if meta exists)
        public decimal? WeightProgress { get; set; }
        public decimal? BodyFatProgress { get; set; }
        public decimal? CholesterolProgress { get; set; }
        public decimal? BMIProgress { get; set; }
        public decimal? MuscleMassProgress { get; set; }
    }
}
