using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public class Activity_ {

        public int Activity_Id { get; set; }
        [Required]
        public string Activity_Name { get; set; }
        public string? Activity_Description { get; set; }
        [Required]
        public string Activity_Type { get; set; }
        public int? NumberSets { get; set; }
        public int? NumberReps { get; set; }
        public decimal Weigth { get; set; }
    }
}
