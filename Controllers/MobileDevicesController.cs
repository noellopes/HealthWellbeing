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
            return View(new MobileDevices());
        }

        public IActionResult ReadMobileDevices(int id)
        {
            // Procura na lista, o dispositivo com o ID recebido
            var dispositivo = RepositoryMobileDevices.Index.FirstOrDefault(d => d.DevicesID == id);

            return View(dispositivo);
        }

        public IActionResult UpdateMobileDevices(int id)
        {
            var dispositivo = RepositoryMobileDevices.Index.FirstOrDefault(d => d.DevicesID == id);

            // Envia o registo encontrado para a View para pré-preenchimento
            return View(dispositivo);
        }

        public IActionResult DeleteMobileDevices(int id)
        {
            var dispositivo = RepositoryMobileDevices.Index.FirstOrDefault(d => d.DevicesID == id);

            // Envia o dispositivo para a View para o utilizador confirmar qual item será apagado
            return View(dispositivo);
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

                TempData["Mensagem de sucesso"] = $"Dispositivo '{dispositivo.NomeDisp}' Registado com sucesso!";

                //Retorna a view CreateMobile com todos os campos vazios
                return View(new MobileDevices());
            }

            //Se a validação falhar, mostra a View MobileDevices
            return View(dispositivo);
        }


        //2-> ReadMobileDevices
        [HttpPost]
        public IActionResult UpdateMobileDevices(MobileDevices dispositivo)
        {
            // Verifica as regras de validação
            if (ModelState.IsValid)
            {
                //Chama o método do Repositório para atualizar o item na lista
                RepositoryMobileDevices.UpdateMobileDevices(dispositivo);

                //Redireciona para a lista de dispositivos móveis
                return RedirectToAction("Index");
            }

            //Se a validação falhar, mostra a View MobileDevices
            return View(dispositivo);
        }


        //3-> DeleteMobileDevices
        [HttpPost]
        [ActionName("DeleteMobileDevices")] //liga a ação
        public IActionResult DeleteConfirmed(int id)
        {
            //Chama o método do Repositório para remover o item
            RepositoryMobileDevices.DeleteMobileDevices(id);

            // Redireciona de volta para a lista após a eliminação
            return RedirectToAction("Index");
        }
    }
}
