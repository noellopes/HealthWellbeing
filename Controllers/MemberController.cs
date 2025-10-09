using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HealthWellbeing.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Member/CreateMember
        [HttpGet]
        public IActionResult CreateMember()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateMember(Member member)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Models.Repository.AddMember(member);

            return View("CreateMemberComplete", member);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
