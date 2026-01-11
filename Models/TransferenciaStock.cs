using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models 
{
    
    public class TransferenciaStock
    {
        public int ZonaOrigemId { get; set; }
        public string? ZonaOrigemNome { get; set; }
        public string? ConsumivelNome { get; set; }
        public int QuantidadeAtualOrigem { get; set; }

        [Required(ErrorMessage = "Selecione a zona de destino.")]
        [Display(Name = "Zona de Destino")]
        public int ZonaDestinoId { get; set; }

        [Required(ErrorMessage = "Insira a quantidade a transferir.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser superior a 0.")]
        [Display(Name = "Quantidade a Transferir")]
        public int QuantidadeATransferir { get; set; }
    }
}