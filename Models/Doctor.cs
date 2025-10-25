using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models{

    public class Doctor{
        [Required(ErrorMessage ="Por favor introduza o id do Médico")]
        [RegularExpression(@"^[A-Z]{2,4}-?\d{3,5}$", ErrorMessage = "O id dp Médico deve ter o formato XXX-12345")]
        public int IdMedico { get; set; } = default!;

        [Required(ErrorMessage = "Por favor introduza o nome do Médico")]
        [StringLength(50,MinimumLength = 3,ErrorMessage = "O nome do médico deve ter entre 3 a 50 caracteres")]
        public string Nome { get; set; } = default!;

        [Required(ErrorMessage = "Por favor introduza o número de telemóvel")]
        [Phone(ErrorMessage = "Número de telemóvel inválido")]
        public string Telemovel { get; set; } = default!;

        [Required(ErrorMessage = "Por favor introduza o email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = default!;

    }
}
