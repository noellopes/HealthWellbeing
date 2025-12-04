namespace HealthWellbeing.Models
{
    public class UtenteGrupo7
    {
        public int UtenteGrupo7Id { get; set; }

        public string Nome { get; set; }

        public ICollection<Sono>? Sonos { get; set; }

        public int? ObjetivoFisicoId { get; set; }
        public ObjetivoFisico? ObjetivoFisico { get; set; }

        public ICollection<UtenteGrupo7ProblemaSaude>? UtenteProblemasSaude { get; set; }
    }
}
