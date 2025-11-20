using HealthWellBeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Funcao
    {
        
        public int FuncaoId { get; set; }

       
        [Required(ErrorMessage = "O nome da função é obrigatório")]
        [StringLength(150)]
        public string NomeFuncao { get; set; }

    
        public ICollection<ProfissionalExecutante> ProfissionaisExecutantes { get; set; }
    }
}
