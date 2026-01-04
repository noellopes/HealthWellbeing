using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HealthWellbeing.Models
{
    public class AgendaMedica
    {
        [Key]public int IdAgendaMedica { get; set; }

        
        [ForeignKey(nameof(Medico))]
        public int? IdMedico { get; set; }
        public Doctor? Medico { get; set; }

        public DateOnly Data { get; set; }

        public DayOfWeek DiaSemana { get; set; }

        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFim { get; set; }
        public string Periodo { get; set; } = "";
    }
}
