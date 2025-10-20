using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UtenteBalneario
    {
        [Key]
        //Infos Utente
        public int UtenteBalnearioId { get; set; }
        public string Nome { get; set; }

        public DateTime DataNascimento { get; set; }

        public string Sexo { get; set; }

        public string NIF { get; set; }

        public string Contacto { get; set; }

        public string Morada { get; set; }

        //DadosMedicos
        public string HistoricoClinico { get; set; }

        public string IndicacoesTerapeuticas { get; set; }

        public string ContraIndicacoes { get; set; }

        public string MedicoResponsavel { get; set; }



        //Dados administrativos
        public DateTime DataInscricao { get; set; }

        public string SeguroSaude { get; set; }

        public bool Ativo { get; set; }

    }
}
