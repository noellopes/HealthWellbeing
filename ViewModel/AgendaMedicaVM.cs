using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.ViewModel
{
    public class MedicoAgendaVM
    {
        public string Periodo { get; set; } = "";

        public bool Ativo { get; set; } = true;

        [Required]
        public TimeOnly HoraInicio { get; set; }

        [Required]
        public TimeOnly HoraFim { get; set; }
    }

    public class AgendaDiaVM
    {
        public DateOnly Data { get; set; }
        public DayOfWeek DiaSemana { get; set; }

        public MedicoAgendaVM Manha { get; set; } = new MedicoAgendaVM
        {
            Periodo = "Manha",
            HoraInicio = new TimeOnly(9, 0),
            HoraFim = new TimeOnly(12, 0),
            Ativo = true
        };

        public MedicoAgendaVM Tarde { get; set; } = new MedicoAgendaVM
        {
            Periodo = "Tarde",
            HoraInicio = new TimeOnly(14, 0),
            HoraFim = new TimeOnly(17, 0),
            Ativo = true
        };
    }

    public class MedicoAgenda15DiasVM
    {
        public string Medico { get; set; } = "";
        public List<AgendaDiaVM> Dias { get; set; } = new();
    }

    public class EspecialidadeAgendaVM
    {
        public string Especialidade { get; set; } = "";

        public List<DateOnly> Datas { get; set; } = new();

        public List<MedicoAgendaTabelaVM> Medicos { get; set; } = new();
    }

    public class MedicoAgendaTabelaVM
    {
        public string Medico { get; set; } = "";

        public Dictionary<DateOnly, string> HorariosPorData { get; set; } = new();
    }
}