namespace HealthWellbeing.ViewModel
{
    public class MarcarConsultaGridVM
    {

        public int? IdEspecialidade { get; set; }
        public int? IdMedico { get; set; }

        public List<DateOnly> Datas { get; set; } = new();
        public List<TimeSlotRowVM> Linhas { get; set; } = new();

        // dropdowns
        public List<SimpleOptionVM> Especialidades { get; set; } = new();
        public List<SimpleOptionVM> Medicos { get; set; } = new();

        public bool NaoSelecionarMedico { get; set; }  // checkbox

        // slot escolhido (vai ser preenchido na view via inputs hidden)
        public string? DataSelecionada { get; set; }       // "yyyy-MM-dd"
        public string? HoraInicioSelecionada { get; set; } // "HH:mm"
        public string? HoraFimSelecionada { get; set; }    // "HH:mm"
    }

    public class SimpleOptionVM
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
    }

    public class TimeSlotRowVM
    {
        public TimeOnly Inicio { get; set; }
        public TimeOnly Fim { get; set; }
        public string Label => $"{Inicio:HH\\:mm}-{Fim:HH\\:mm}";

        // uma célula por data
        public List<TimeSlotCellVM> Cells { get; set; } = new();
    }

    public class TimeSlotCellVM
    {
        public DateOnly Data { get; set; }
        public TimeOnly Inicio { get; set; }
        public TimeOnly Fim { get; set; }

        // "livre" | "ocupado" | "fora"
        public string Estado { get; set; } = "fora";
    }
}
