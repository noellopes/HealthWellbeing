using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class ComponentesDaReceita
    {
        [Key]
        public int ComponentesDaReceitaId { get; set; }

        [Required]
        [ForeignKey(nameof(Receita))]
        [Display(Name = "ID da Receita")]
        public int ReceitaId { get; set; }

        public Receita? Receita { get; set; }

        [Required(ErrorMessage = "O nome do componente e obrigatorio.")]
        [StringLength(120, ErrorMessage = "O nome deve ter no maximo 120 caracteres.")]
        [Display(Name = "Nome do Componente")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(1024, ErrorMessage = "A descricao deve ter no maximo 1024 caracteres.")]
        [Display(Name = "Descricao")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A unidade de medida e obrigatoria.")]
        [StringLength(32, ErrorMessage = "A unidade de medida deve ter no maximo 32 caracteres.")]
        [Display(Name = "Unidade de Medida (ex.: g, ml, un)")]
        public string UnidadeMedida { get; set; } = string.Empty;

        [Display(Name = "Quantidade")]
        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "A quantidade deve ser maior que zero.")]
        public decimal Quantidade { get; set; }

        [Display(Name = "Calorias")]
        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "As calorias nao podem ser negativas.")]
        public decimal Calorias { get; set; }

        [Display(Name = "Proteinas (g)")]
        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "As proteinas nao podem ser negativas.")]
        public decimal Proteinas { get; set; }

        [Display(Name = "Hidratos de Carbono (g)")]
        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "Os hidratos de carbono nao podem ser negativos.")]
        public decimal HidratosCarbono { get; set; }

        [Display(Name = "Gorduras (g)")]
        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "As gorduras nao podem ser negativas.")]
        public decimal Gorduras { get; set; }

        [Display(Name = "E opcional?")]
        public bool IsOpcional { get; set; } = false;
    }
}
