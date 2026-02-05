using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class HorarioTerapeuta
    {
        [Key]
        public int HorarioTerapeutaId { get; set; }

        [Required]
        public DateTime DataHoraInicio { get; set; }

        [Required]
        public DateTime DataHoraFim { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Disponível";

        // Terapeuta
        [Required]
        public int TerapeutaId { get; set; }
        public Terapeuta Terapeuta { get; set; }

        // Especialidade (tratamento)
        [Required]
        public int EspecialidadeId { get; set; }
        public Especialidade Especialidade { get; set; }
    }
}