namespace HealthWellbeing.Models
{
    public class ClientAlergia
    {

        public string ClientId { get; set; } 

        public Client? Client { get; set; }

        public int AlergiaId { get; set; }

        public Alergia? Alergia { get; set; }
    }
}
