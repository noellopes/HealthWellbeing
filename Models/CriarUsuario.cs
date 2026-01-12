using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.ViewModels
{
    public class CriarUsuario
    {
        public string? Id { get; set; } // Necessário para o Index

        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Password é obrigatória")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Password")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Role { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}