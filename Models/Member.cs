using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Member
    {   
        public int MemberId { get; set; } = default!;

        [Key, ForeignKey("Client")]
        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;
    }
}
