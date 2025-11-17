using HealthWellbeingRoom.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Room
    {
        // Chave primária
        [Key]
        public int RoomId { get; set; }

        // ID formatado com 3 dígitos (ex: 001)
        [NotMapped]
        public string FormattedRoomId => RoomId.ToString("D3");


        // Especialidade médica
        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        [StringLength(100, ErrorMessage = "A especialidade não pode exceder 100 caracteres.")]
        public string Specialty { get; set; }

        // Nome da sala
        [Required(ErrorMessage = "O nome da sala é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Name { get; set; }

        // Localização física
        [Required(ErrorMessage = "A localização é obrigatória.")]
        public string Location { get; set; }

        // Horário de funcionamento
        [Required(ErrorMessage = "O horário de funcionamento é obrigatório.")]
        public string OperatingHours { get; set; }

        // Enum para estado da sala
        public enum RoomStatus
        {
            [Display(Name = "Criado")]
            Criado,
            [Display(Name = "Disponível")]
            Disponivel,
            [Display(Name = "Indisponível")]
            Indisponivel,
            [Display(Name = "Limpeza")]
            Limpeza,
            [Display(Name = "Manutenção")]
            Manutencao,
            [Display(Name = "Fora de serviço")]
            ForaDeServico
        }

        public RoomStatus Status { get; set; } = RoomStatus.Criado;

        // Observações
        public string? Notes { get; set; }

        // Relação com tabela intermediária
        public ICollection<LocationMedDevice> LocalizacaoDispMedicoMovel { get; set; } = new List<LocationMedDevice>();
    }
}