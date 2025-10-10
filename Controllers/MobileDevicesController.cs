using HealthWellbeingRoom.Models.FileMobileDevices;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeingRoom.Controllers
{
    public class MobileDevicesController : Controller
    {
        //Apresenta o formulário(get)
        [HttpGet]
        public IActionResult CreateMobileDevices()
        {
            return View(new MobileDevices {});
        }

        public IActionResult ReadMobileDevices()
        {
            return View();
        }

        public IActionResult UpdateMobileDevices()
        {
            return View();
        }

        public IActionResult DeleteMobileDevices()
        {
            return View();
        }
        public IActionResult Index() //Controller da lista dos dispositivos móveis
        {
            //Pega a lista de dispositivos no Repositório.
            var listaDeDispositivos = RepositoryMobileDevices.Index;

            //Mostra a lista de dispositivos -> Index.cshtml.
            return View(listaDeDispositivos);
        }

        public IActionResult Dashboard() //Controller do painel dos Dispositivos Móveis
        {
            //Mostra o painel dos Dispositivos Móveis
            return View("DashboardMobileDevices");
        }

        //----------------------------------------------------Processa a submissão(POST)
        //1-> CreateMobileDevices
        [HttpPost]
        public IActionResult CreateMobileDevices(MobileDevices dispositivo)
        {
            if (ModelState.IsValid)
            {
                //Obtém a lista de dispositivos móveis
                var listaAtual = RepositoryMobileDevices.Index;

                //Gera novo id sequencial começando de 1
                dispositivo.DevicesID = listaAtual.Count() + 1;

                //Regista data e hora da criação
                dispositivo.DataRegisto = DateTime.Now;

                //Guarda dados
                RepositoryMobileDevices.AddMobileDevices(dispositivo);

                //Redireciona para a lista de dispositivos móveis
                return RedirectToAction("Index");
            }
            // Se a validação falhar, mostra a View MobileDevices
            return View(dispositivo);
        }
    }
}
