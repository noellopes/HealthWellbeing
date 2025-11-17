using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Utils.Group1.DTOs
{
    public class TreatmentRecordListDTO
    {
        public required int Id { get; set; }

        [Display(Name = "Enfermeiro(a) Responsável")]
        public string NurseName { get; set; }

        [Display(Name = "Tipo de tratamento")]
        public string TreatmentTypeName { get; set; }

        [Display(Name = "Patologia associada")]
        public string PathologyName { get; set; }

        public DateTime TreatmentDate { get; set; }
        public int? CompletedDuration { get; set; }
        public string Observations { get; set; }
        public string AdditionalNotes { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
