using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HealthWellBeing.Models
{
    public class Utente
    {
        public int UtenteId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O número de utente de saúde é obrigatório.")]
        [StringLength(20)]
        public string NumeroUtente { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        [EmailAddress(ErrorMessage = "Endereço de e-mail inválido.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido.")]
        public string Telefone { get; set; }

        // Propriedade de navegação
        //public ICollection<Exame> Exames { get; set; }, base de dados ainda nao esta
    }
}
