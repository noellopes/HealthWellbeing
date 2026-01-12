using System;
using System.Collections.Generic;
using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModels
{
    public class ProgressReportViewModel
    {
        // Filtros
        public int MemberId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        // Resultados
        public int TotalTrainingsAttended { get; set; }

        public List<PhysicalAssessment> Assessments { get; set; }
            = new List<PhysicalAssessment>();
    }
}
