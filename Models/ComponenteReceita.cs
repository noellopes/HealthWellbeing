using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public enum UnidadeMedidaEnum
    {
        Grama,
        Mililitro,
        Unidade,
        ColherDeSopa,
        ColherDeCha,
        Xicara,

        Fatia,
    }

    public class ComponenteReceita
    {
        public int ComponenteReceitaId { get; set; }

        [Required]
        public int AlimentoId { get; set; }
        public Alimento? Alimento { get; set; }

        [Required]
        [Display(Name = "Receita")]
        public int ReceitaId { get; set; }
        public Receita? Receita { get; set; }

        [Required]
        [Display(Name = "Unidade")]
        public UnidadeMedidaEnum UnidadeMedida { get; set; }

        [Range(1, 9999999999)]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Display(Name = "É opcional?")]
        public bool IsOpcional { get; set; } = false;
    }
}
