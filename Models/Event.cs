using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "O nome do evento é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do evento não pode exceder 100 caracteres.")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição do evento é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição do evento não pode exceder 500 caracteres.")]
        public string EventDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de evento é obrigatório.")]
        [StringLength(50, ErrorMessage = "O tipo de evento não pode exceder 50 caracteres.")]
        public string EventType { get; set; } = string.Empty;

        [Range(1, 600, ErrorMessage = "A duração deve estar entre 1 e 600 minutos.")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "A intensidade é obrigatória.")]
        [StringLength(20, ErrorMessage = "A intensidade não pode exceder 20 caracteres.")]
        public string Intensity { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Data do Evento")]
        public DateTime EventDate { get; set; } = DateTime.Now;
    }
}
