using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Servico
    {
        // CORREÇÃO: ADICIONAR CONSTRUTOR VAZIO AQUI
        public Servico() { }

        [Key]
        public int ServicoId { get; set; }

        [Required(ErrorMessage = "O nome do serviço é obrigatório.")]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(10, 2)")] 
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A duração em minutos é obrigatória.")]
        public int DuracaoMinutos { get; set; }

        
        public int? TipoServicoId { get; set; }

        [ForeignKey("TipoServicoId")]
        public TipoServico TipoServico { get; set; }

        
    }
}