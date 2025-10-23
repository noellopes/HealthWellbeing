using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Exames
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A instância do exame é obrigatória")]
        [StringLength(100)]
        public string Instancia { get; set; }

        [StringLength(500)]
        public string data { get; set; }
        public string hora { get; set; }

        public ExameTipo exameTipo { get; set; }

       //public Medicos medicos { get; set; }






    }
}
