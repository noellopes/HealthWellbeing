using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Models
{
    public class Profissional
    {
        public int Id { get; set; }

        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string NumeroCedula { get; set; }
        public string UserId { get; set; }
    }
}