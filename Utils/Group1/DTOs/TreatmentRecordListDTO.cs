using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Utils.Group1.DTOs
{
    public class TreatmentRecordListDTO
    {
        public required int Id { get; set; }

        [Display(Name = "Enfermeiro(a) Responsável")]
        public string Nurse { get; set; }

        [Display(Name = "Tipo de tratamento")]
        public string TreatmentType { get; set; }

        [Display(Name = "Patologia associada")]
        public string Pathology { get; set; }

        [Display(Name = "Data pretendida para o tratamento")]
        public DateTime TreatmentDate { get; set; }

        [Display(Name = "Duração total do tratamento")]
        public int? CompletedDuration { get; set; }

        [Display(Name = "Observações")]
        public string Observations { get; set; }

        [Display(Name = "Notas adicionais")]
        public string AdditionalNotes { get; set; }

        [Display(Name = "Estado do tratamento")]
        public string Status { get; set; }

        [Display(Name = "Data de submissão")]
        public DateTime CreatedAt { get; set; }
    }
}
