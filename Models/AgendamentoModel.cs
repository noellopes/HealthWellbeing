namespace HealthWellbeing.Models
{
    public class AgendamentoModel
    {
        public int AgendamentoId { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string Estado { get; set; }// "Pendente", "Confirmado", "Cancelado"

        public int UtenteBalnearioId { get; set; }
        public UtenteBalneario UtenteBalneario { get; set; }
        public int TerapeutaId {  get; set; }
        public TerapeutaModel Terapeuta { get; set; }
        public int ServicoId { get; set; }  
        public ServicoModel Servico { get; set; } 


    }
}
