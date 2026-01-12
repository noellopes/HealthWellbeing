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
        public IActionResult Index()
        {
            var vm = new ProgressReport
            {
                From = DateTime.Today.AddMonths(-1),
                To = DateTime.Today
            };

            return View(vm);
        }

        // POST: ProgressReport
        [HttpPost]
        public IActionResult Index(ProgressReport vm)
        {
            // ⚠️ EXEMPLO SIMPLES
            // depois podes trocar pelo member logado
            int memberId = 1;

            // 1️⃣ Planos do membro no período
            var memberPlanIds = _context.MemberPlan
                .Where(mp =>
                    mp.MemberId == memberId &&
                    mp.StartDate >= vm.From &&
                    mp.StartDate <= vm.To)
                .Select(mp => mp.PlanId)
                .ToList();

            // 2️⃣ Número de treinos associados a esses planos
            vm.TotalTrainingsAttended = _context.TrainingPlan
                .Where(tp => memberPlanIds.Contains(tp.PlanId))
                .Count();

            // 3️⃣ Avaliações físicas no período
            vm.Assessments = _context.PhysicalAssessment
                .Include(pa => pa.Trainer)
                .Where(pa =>
                    pa.MemberId == memberId &&
                    pa.AssessmentDate >= vm.From &&
                    pa.AssessmentDate <= vm.To)
                .OrderBy(pa => pa.AssessmentDate)
                .ToList();

            return View(vm);
        }
    }
}
