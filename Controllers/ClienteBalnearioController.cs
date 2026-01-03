using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class ClienteBalnearioController : Controller
    {
        public IActionResult Index()
        {
            var clientes = new List<ClienteBalnearioModel>
        {
            new ClienteBalnearioModel { Nome = "Teste", Email = "teste@email.com" }
        };

            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }
    }

}
