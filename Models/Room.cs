using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Room
    {
        // ID da sala, chave primária
        public int RoomId { get; set; }

        // Tipo da sala (consultas ou tratamentos), não precisa de ser validado
        public enum RoomType { Consultas, Tratamentos }

        [Required(ErrorMessage = "O tipo de sala é obrigatório.")]
        public RoomType RoomsType { get; set; }

        // Especialidade da sala (pediatria, cardiologia, etc)
        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        [StringLength(100, ErrorMessage = "A especialidade não pode exceder 100 caracteres.")]
        public string Specialty { get; set; }

        // Nome da sala
        [Required(ErrorMessage = "O nome da sala é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Name { get; set; }

        // Capacidade da sala (1 a 50), não precisa de ser validado
        public int Capacity { get; set; }

        // Localização da sala (piso, ala, etc)
        [Required(ErrorMessage = "A localização é obrigatória.")]
        public string Location { get; set; }

        // Horário de funcionamento da sala (ex: 08:00 - 18:00)
        [Required(ErrorMessage = "O horário de funcionamento é obrigatório.")]
        public string OperatingHours { get; set; }

        // Estado da sala (disponível, indisponível, limpeza, manutenção, fora de serviço)
        public enum RoomStatus
        {
            Disponivel,
            Indisponivel,
            Limpeza,
            Manutencao,
            ForaDeServico
        }
        public RoomStatus Status { get; set; }

        // Observações adicionais (máximo 500 caracteres)
        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
        public string Notes { get; set; }
    }
}