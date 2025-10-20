using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class MaterialEquipamentoAssociado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do equipamento é obrigatório.")]
        [StringLength(100)]
        public string Nome_do_Equipamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser um número positivo.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O estado do componente é obrigatório (ex.: operacional, avariado, em manutenção).")]
        [StringLength(50)]
        public string Estado_do_componente { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Aparelhos_de_imagiologia { get; set; }

        [StringLength(100)]
        public string? Reagentes { get; set; }
    }
}
