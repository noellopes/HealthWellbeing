using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HealthWellbeing.Models
{
    public class TipoServicos
    {
        [Key]
        public int TipoServicosId { get; set; }

        [Required(ErrorMessage = "O nome do tipo de serviço é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }// Ex.: "massagens", "banhos", "tratamentos", "fisioterapia", "Programas de bem-estar"

        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }// Detalhes do tipo de serviço

        
    }
}
