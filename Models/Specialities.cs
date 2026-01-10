using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Specialities
    {
        [Key]
        public int IdEspecialidade { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(80, ErrorMessage = "Máximo 80 caracteres.")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(1500, ErrorMessage = "Máximo 1500 caracteres.")]
        public string Descricao { get; set; } = "";

        
        public ICollection<Consulta>? Consultas { get; set; }

        
        public ICollection<Doctor>? Medicos { get; set; }

        [MaxLength(5000)]
        public string? OqueEDescricao { get; set; }

        public ICollection<SpecialitiesDoctor>? SpecialitiesDoctors { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
