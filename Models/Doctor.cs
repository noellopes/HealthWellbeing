using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models{

    public class Doctor{
        [Required(ErrorMessage ="Por favor introduza o id do Médico")]
        [Key] public int IdMedico { get; set; } = default!;

        [Required(ErrorMessage = "Por favor introduza o nome do Médico")]
        [StringLength(50,MinimumLength = 3,ErrorMessage = "O nome do médico deve ter entre 3 a 50 caracteres")]
        public string Nome { get; set; } = default!;

        [Required(ErrorMessage = "Por favor introduza o número de telemóvel")]
        [RegularExpression(@"^9[1236]\d{7}$",ErrorMessage = "O número de telemóvel deve ser português, começar por 91, 92, 93 ou 96 e ter 9 dígitos")]
        [Phone(ErrorMessage = "Número de telemóvel inválido")]
        public string Telemovel { get; set; } = default!;

        [Required(ErrorMessage = "Por favor introduza o email")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",ErrorMessage = "Por favor introduza um email válido")]
        [EmailAddress(ErrorMessage = "Email inválido")]

        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Por favor selecione a especialidade")]
        [ForeignKey(nameof(Especialidade))]
        public int IdEspecialidade { get; set; }
        public Specialities Especialidade { get; set; } = default!;

    }
}
