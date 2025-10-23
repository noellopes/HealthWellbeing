using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Exame
    {
        [Key]
        public int ExameId { get; set; }

        [Required(ErrorMessage = "O nome do utente é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do utente não pode ter mais de 100 caracteres.")]
        [Display(Name = "Nome do Utente")]
        public string NomeUtente { get; set; }

        [Required(ErrorMessage = "A data do exame é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data do Exame")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "O tipo de exame é obrigatório.")]
        [StringLength(50)]
        [Display(Name = "Tipo de Exame")]
        public string TipoExame { get; set; }

        [StringLength(200)]
        [Display(Name = "Observações")]
        public string Observacoes { get; set; }
    }
}
