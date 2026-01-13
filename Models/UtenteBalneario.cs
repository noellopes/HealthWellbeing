using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UtenteBalneario
    {
        [Key]
        public int UtenteBalnearioId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O sexo é obrigatório")]
        public  Sexo Sexo { get; set; }

        [StringLength(9, ErrorMessage = "O NIF deve ter 9 caracteres.")]
        public string NIF { get; set; } = string.Empty;

        [Phone]
        [StringLength(15)]
        public string Contacto { get; set; } = string.Empty;

        [StringLength(200)]
        public string Morada { get; set; } = string.Empty;



        //DadosMedicos
        public DadosMedicos? DadosMedicos { get; set; } = new DadosMedicos();


        //Dados administrativos
        [Required]
        [DataType(DataType.Date)]
        public DateTime DataInscricao { get; set; }

        public  SeguroSaude? SeguroSaude { get; set; } = new SeguroSaude();

        public bool Ativo { get; set; }

    }

        public class DadosMedicos
        
        {
            [Key]
             public int DadosMedicosId { get; set; }
             
             public string HistoricoClinico { get; set; } = string.Empty;

             public string IndicacoesTerapeuticas { get; set; } = string.Empty;

             public  string ContraIndicacoes {  get; set; } = string.Empty;

            [StringLength(100)]
             public string MedicoResponsavel { get; set; } = string.Empty;
    }
        public class SeguroSaude

        {
             [Key]
              public int SeguroSaudeId { get; set; }

             [Required(ErrorMessage = "O nome da seguradora é obrigatório")]
             [StringLength(100)]
              public string NomeSeguradora { get; set; } = string.Empty;

             [StringLength(50)]
              public string NumeroApolice { get; set; } = string.Empty;
    }
}
