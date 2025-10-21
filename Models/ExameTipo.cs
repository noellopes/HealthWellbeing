using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ExameTipo : Controller
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome do exame é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string Descricao { get; set; }
        

        

    }

  
}

