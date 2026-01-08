using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class PlanoAlimentar
    {
        [Key]
        public int PlanoAlimentarId { get; set; }

        [Required(ErrorMessage = "Client is required.")]
        public string ClientId { get; set; }
        public Client? Client { get; set; }

        [Required(ErrorMessage = "Goal is required.")]
        public int MetaId { get; set; }
        public Meta? Meta { get; set; }
        public ICollection<Receita> Receitas { get; set; } = new List<Receita>();

    }
}