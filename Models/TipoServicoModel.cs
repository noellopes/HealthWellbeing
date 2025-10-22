using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TipoServicoModel
    {
        [Key]
        public int TipoServicoId { get; set; }   

        [Required]
        public string Nome { get; set; }         // Ex.: "Massagem", "Hidroterapia", "Estética", "Fisioterapia"
        public string Descricao { get; set; }    // Detalhes do tipo de serviço

        // Relacionamento 1:N com os serviços específicos
        public ICollection<ServicoModel> Servicos { get; set; }
    }
}
