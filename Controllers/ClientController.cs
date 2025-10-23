using System.Diagnostics;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class ClientController : Controller
    {
		// GET: /Client/Index
		public IActionResult Index()
        {
            return View();
        }
		// GET: /Client/Create
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Client client)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			Models.Repository.AddClient(client);

			return View("CreateComplete", client);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
