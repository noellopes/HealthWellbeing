using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProfissionalExecutante
    {
        [Key]
        public int ProfissionalExecutanteId { get; set; } // Chave primária

        public string Nome { get; set; }
        public string Funcao { get; set; } // Futuramente poderá ser uma entidade (e.g. tabela Função)

        public string Telefone { get; set; }
        public string Email { get; set; }

        public virtual ICollection<ProblemaSaude> ProblemasSaude { get; set; }

        public ProfissionalExecutante()
        {
            ProblemasSaude = new HashSet<ProblemaSaude>();
        }
    }
}
