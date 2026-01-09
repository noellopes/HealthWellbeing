using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ClientAlergy
    {
        [Key]
        public int ClientAlergyId { get; set; }

        [Required(ErrorMessage = "Client is required.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Allergy is required.")]
        public int AlergyId { get; set; }

        public Client? Client { get; set; }
        public Alergy? Alergy { get; set; }
    }
}
