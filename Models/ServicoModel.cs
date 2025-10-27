using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class ServicoModel
    {
        [Key]
        public int ServicoId { get; set; }

        [Required]
        public string Nome { get; set; } 
        public string Descricao { get; set; }
        [Required]
        public decimal Preco { get; set; } 
        [Required]
        public int DuracaoMinutos { get; set; } 


        [Required]
        public int TipoServicoId { get; set; }

        [ForeignKey("TipoServicoId")]
        public TipoServico TipoServico { get; set; }

        // Relação: um serviço pode estar associado a vários agendamentos
        public ICollection<AgendamentoModel> Agendamentos { get; set; }
    }
}
