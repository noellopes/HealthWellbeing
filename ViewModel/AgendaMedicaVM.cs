using System;
using System.Collections.Generic;

namespace HealthWellbeing.ViewModel
{
    public class MedicoAgendaVM
    {
        public string Medico { get; set; } = "";
        public string Segunda { get; set; } = "";
        public string Terca { get; set; } = "";
        public string Quarta { get; set; } = "";
        public string Quinta { get; set; } = "";
        public string Sexta { get; set; } = "";
    }

    public class EspecialidadeAgendaVM
    {
        public string Especialidade { get; set; } = "";
        public List<MedicoAgendaVM> Medicos { get; set; } = new();
    }
}