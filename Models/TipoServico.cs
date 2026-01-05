using System.ComponentModel.DataAnnotations;


namespace HealthWellbeing.Models
{
    public class TipoServicos
    {
        [Key]
        public int TipoServicosId { get; set; }

        [Required(ErrorMessage = "O nome do tipo de serviço é obrigatório.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "O nome não pode conter números ou caracteres especiais.")]
        [StringLength(100, ErrorMessage = "O nome é demasiado longo.")]
        public string Nome { get; set; }// Ex.: "massagens", "banhos", "tratamentos", "fisioterapia", "Programas de bem-estar"

        [Display(Name = "Descrição")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "A descrição não pode conter números ou caracteres especiais.")]
        public string? Descricao { get; set; }// Detalhes do tipo de serviço

        
    }
}
