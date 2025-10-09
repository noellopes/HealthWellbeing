using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Diagnostics;

namespace HealthWellbeing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult TypeTreatmentRegister()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TypeTreatmentRegister(TypeTreatment typeTreatment)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            RepositoryTypeTreatment.AddTypeTreatment(typeTreatment);

            return View("TypeTreatmentRegisterComplete", typeTreatment);


        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
