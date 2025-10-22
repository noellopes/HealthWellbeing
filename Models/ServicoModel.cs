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

        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; } // Valor do serviço

        [Range(1, 120)] // Duração entre 1 e 120 minutos (2h)
        public int DuracaoMinutos { get; set; } // Duração média do serviço

        [Required]
        public int TipoServicoId { get; set; }

        [ForeignKey("TipoServicoId")]
        public TipoServicoModel TipoServico { get; set; }

        // Relação: um serviço pode estar associado a vários agendamentos
        public ICollection<AgendamentoModel> Agendamentos { get; set; }
    }
}
