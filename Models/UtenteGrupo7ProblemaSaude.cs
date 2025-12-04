namespace HealthWellbeing.Models
{
    public class UtenteGrupo7ProblemaSaude
    {
        public int UtenteGrupo7Id { get; set; }
        public UtenteGrupo7 Utente { get; set; }

        public int ProblemaSaudeId { get; set; }
        public ProblemaSaude ProblemaSaude { get; set; }
    }
}
