namespace HealthWellbeing.Models
{
    public class UtentesSaude
    {
        public string Nome { get; set; } = default!;            // Nome completo
        public DateTime DataDeNascimento { get; set; } = default!;  // Usando DateTime para manipulação de datas para poder marcar a data de nascimento
        public int NIF { get; set; } = default!;            // Identificação fiscal 
        public int NSS { get; set; } = default!;   // Numero de segurança social
        public int NUS { get; set; } = default!;   // Numero do Utente de Saude
        public int Idade { get; set; } = default!;              // ter em conta que pode ser calculada a partir da DataDeNascimento 
        public string Email { get; set; } = default!;          // Email como string
        public string Telefone { get; set; } = default!;      // String para preservar zeros iniciais e códigos
        public string Morada { get; set; } = default!;          // Endereço
    }
}
