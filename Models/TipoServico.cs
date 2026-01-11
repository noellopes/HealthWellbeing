using System.ComponentModel.DataAnnotations;


namespace HealthWellbeing.Models
{
    public class TipoServicos
    {
        [Key]
        public int TipoServicosId { get; set; }

        [Required(ErrorMessage = "O nome do tipo de serviço é obrigatório.")]
        [RegularExpression(@"^[^0-9]+$", ErrorMessage = "O nome não pode conter números.")]
        [StringLength(100, ErrorMessage = "O nome é demasiado longo.")]
        public string Nome { get; set; }// Ex.: "massagens", "banhos", "tratamentos", "fisioterapia", "Programas de bem-estar"

        [Display(Name = "Descrição")]
        [RegularExpression(@"^[^0-9]+$", ErrorMessage = "A descrição não pode conter números.")]
        public string? Descricao { get; set; }// Detalhes do tipo de serviço

        
    }
}
