using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Member
    {   
        public int MemberId { get; set; } = default!;

        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; } = default;
    }
}
