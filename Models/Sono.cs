using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Sono
    {
        public int SonoId { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        [DataType(DataType.Date)] // Garante que o input no browser é apenas Data (sem horas)
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A hora de deitar é obrigatória")]
        public TimeSpan HoraDeitar { get; set; }

        [Required(ErrorMessage = "A hora de levantar é obrigatória")]
        public TimeSpan HoraLevantar { get; set; }  

        public TimeSpan HorasSono { get; set; }

        // Chave Estrangeira para UtenteGrupo7
        public int UtenteGrupo7Id { get; set; }
        public UtenteGrupo7? UtenteGrupo7 { get; set; }
    }
}
