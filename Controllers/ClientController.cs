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
			var clients = Repository.GetAllClients(); // obter lista da "tabela"
			return View(clients);
		}


		// GET: /Client/Create
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}


		// POST: /Client/Create
		[HttpPost]
		public IActionResult Create(Client client)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			Repository.AddClient(client);

			return View("CreateComplete", client);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}

