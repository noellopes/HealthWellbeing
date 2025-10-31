using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Exame
    {
        public int examesId { get; set; }

        [StringLength(100)]
        public string marcacaodeutente { get; set; }

        public DateTime data { get; set; }

        public ExameTipo exameTipo { get; set; }

        //public Medicos medicos { get; set; }






    }
}