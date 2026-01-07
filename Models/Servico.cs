using HealthWellbeing.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{

    public class Servico
{
    [Key]
    public int ServicoId { get; set; }

    [Required(ErrorMessage = "O nome do serviço é obrigatório.")]
    [MaxLength(100)]
    public string Nome { get; set; }

    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório.")]
    [Column(TypeName = "decimal(18,2)")]

        [Display(Name = "Preço(€)")]
        public decimal Preco { get; set; }

    [Required(ErrorMessage = "A duração é obrigatória.")]

        [Display(Name = "Duração(Minutos)")]
        public int DuracaoMinutos { get; set; }

    
    [Required(ErrorMessage = "O tipo de serviço é obrigatório.")]
    [DisplayName("Tipo de Serviço")]
    public int TipoServicoId { get; set; }

    [ForeignKey("TipoServicoId")]
    public virtual TipoServicos? TipoServico { get; set; } 
}
    }