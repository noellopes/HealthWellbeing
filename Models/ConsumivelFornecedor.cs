using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ConsumivelFornecedor
    {
        [Key]
        public int FornecedorId { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome não pode ter mais de 100 caracteres.")]
        public string NomeEmpresa { get; set; }

        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "O NIF deve ter exatamente 9 dígitos.")]
        public string NIF { get; set; }
        [Required(ErrorMessage = "A Morada é obrigatória.")]
        [StringLength(200, ErrorMessage = "A Morada não pode ter mais de 200 caracteres.")]
        public string Morada { get; set; }

        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        [Phone(ErrorMessage = "O número de telefone não é válido.")]
        [StringLength(13, MinimumLength = 9, ErrorMessage = "O número de telefone deve ter entre 9 e 13 dígitos.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O Email deve conter um formato válido.")]
        public string Email { get; set; }

    }
}