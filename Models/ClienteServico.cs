using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ClienteServico
    {
        [Key]
        public int ClienteServicoId { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(100)]
        public string NomeCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data do serviço é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataServico { get; set; }

        [Required(ErrorMessage = "O tipo de tratamento é obrigatório.")]
        [StringLength(100)]
        public string TipoTratamento { get; set; } = string.Empty;

        [Range(10, 180, ErrorMessage = "A duração deve estar entre 10 e 180 minutos.")]
        public int DuracaoMinutos { get; set; }

        [Required(ErrorMessage = "O nome do terapeuta é obrigatório.")]
        [StringLength(100)]
        public string Terapeuta { get; set; } = string.Empty;

        [StringLength(50)]
        public string Sala { get; set; } = string.Empty;

        [Required]
        public EstadoServico Estado { get; set; } = EstadoServico.Agendado;

        [StringLength(250)]
        public string? Observacoes { get; set; }
    }

    public enum EstadoServico
    {
        Agendado,
        Concluido,
        Cancelado
    }
}
