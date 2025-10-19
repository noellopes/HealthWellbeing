
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Room
    {
        //ID da sala, chave primaria
        public int SalaId { get; set; }

        //Tipo da sala (consultas ou tratamentos), (nao precisa de ser validado)
        public enum Tipo { Consultas, Tratamentos }
        [Required(ErrorMessage = "O tipo de sala é obrigatório.")]
        public Tipo TipoSala { get; set; }

        //Especialidade da sala (pediatria, cardiologia, etc)
        [Required(ErrorMessage = "A especialidade é obrigatoria.")]
        [StringLength(100, ErrorMessage = "A Especialidade não pode exceder 100 caracteres")]
        public string Especialidade { get; set; }

        //Nome da sala
        [Required(ErrorMessage = "O nome da sala é obrigatorio.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        public string Nome { get; set; }

        //Capacidade da sala (1 a 50),(nao precisa de ser validado)
        public int Capacidade { get; set; }

        //Localização da sala (piso, ala, etc)
        [Required(ErrorMessage = "A localização é obrigatoria.")]
        public string Localizacao { get; set; }

        //Lista de agendamentos (datas e horas)
        //public List<string> Agendamentos { get; set; }

        //Disponibilidade da sala (true ou false)
        public bool Disponibilidade { get; set; }

        //Horario de funcionamento da sala (ex: 08:00 - 18:00)
        [Required(ErrorMessage = "O horário de funcionamento é obrigatorio.")]
        public string HorarioFuncionamento { get; set; }

        //Observações adicionais (maximo 500 caracteres)
        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres")]
        public string Observacoes { get; set; }
    }
}
