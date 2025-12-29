using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Servico
    {
        // Construtor vazio necessário para Entity Framework
        public Servico() { }

        [Key]
        public int ServicoId { get; set; }

        [Required(ErrorMessage = "O nome do serviço é obrigatório.")]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Preço (€)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A duração em minutos é obrigatória.")]
        [DisplayName("Duração (minutos)")]
        public int DuracaoMinutos { get; set; }

        [Required(ErrorMessage = "O tipo de serviço é obrigatório.")]
        [DisplayName("Tipo de Serviços")]
        public int TipoServicoId { get; set; }

        [ForeignKey("TipoServicosId")]
        public TipoServicos TipoServico { get; set; }
    }
}
