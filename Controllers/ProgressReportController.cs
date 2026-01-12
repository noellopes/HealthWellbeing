using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class ProgressReportController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ProgressReportController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ProgressReport
        public IActionResult Index(
            int memberId,
            DateTime? from,
            DateTime? to)
        {
            // Datas por defeito (últimos 3 meses)
            DateTime fromDate = from ?? DateTime.Today.AddMonths(-3);
            DateTime toDate = to ?? DateTime.Today;

            // Avaliações físicas do membro no período
            var assessments = _context.PhysicalAssessment
                .Include(a => a.Trainer)
                .Where(a =>
                    a.MemberId == memberId &&
                    a.AssessmentDate >= fromDate &&
                    a.AssessmentDate <= toDate)
                .OrderBy(a => a.AssessmentDate)
                .ToList();

            // Número total de treinos realizados
            // (simplificação aceitável para o trabalho)
            int totalTrainings = _context.MemberPlan
                .Where(mp =>
                mp.MemberId == memberId &&
                mp.StartDate <= toDate &&
                mp.EndDate >= fromDate)
                .Count();


            var report = new ProgressReportViewModel
            {
                MemberId = memberId,
                From = fromDate,
                To = toDate,
                TotalTrainingsAttended = totalTrainings,
                Assessments = assessments
            };

            return View(report);
        }
    }
}
