using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProfissionalExecutante
    {
        [Key]
        public int ProfissionalExecutanteId { get; set; } // Chave primária

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }


        [Required(ErrorMessage = "A função é obrigatória")]
        [StringLength(150)]
        public string Funcao { get; set; } // Futuramente poderá ser uma entidade (e.g. tabela Função)



        [Required(ErrorMessage = "O numero é obrigatório")]
        [StringLength(15)]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [StringLength(250)]
        public string Email { get; set; }

        public ICollection<ProfissionalExecutante>? profissionalExecutantes { get; set; }   

    }
}
