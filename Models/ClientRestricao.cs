namespace HealthWellbeing.Models
{
    public class ClientRestricao
    {

        public string ClientId { get; set; } 

        public Client? Client { get; set; }

        public int RestricaoAlimentarId { get; set; }

        public RestricaoAlimentar? RestricaoAlimentar { get; set; }
    }
}
