using HealthWellbeingRoom.Models.FileMedicalDevices;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeingRoom.Controllers
{
    public class MedicalDevicesController : Controller
    {
        //Apresenta o formulário(get)
        [HttpGet]
        public IActionResult CreateMedicalDevices()
        {
            return View(new MedicalDevices());
        }

        public IActionResult ReadMedicalDevices(int id)
        {
            // Procura na lista, o dispositivo com o ID recebido
            var dispositivo = _repository.devices.FirstOrDefault(d => d.DevicesID == id);
            // 2. VERIFICAÇÃO CRÍTICA: Se o dispositivo não for encontrado, ele é NULL.
            if (dispositivo == null)
            {
                // Boa Prática de ES2: Retornar NotFound (HTTP 404) ou redirecionar para o Index.
                TempData["Mensagem de erro"] = $"Dispositivo com ID {id} não encontrado.";
                return RedirectToAction("Index"); // Volta para a lista
                                                  // return NotFound(); // (Outra opção)
            }

            return View(dispositivo);
        }

        public IActionResult UpdateMedicalDevices(int id)
        {
            var dispositivo = _repository.devices.FirstOrDefault(d => d.DevicesID == id);
            // Envia o registo encontrado para a View para pré-preenchimento
            return View(dispositivo);
        }

        public IActionResult DeleteMedicalDevices(int id)
        {
            var dispositivo = _repository.devices.FirstOrDefault(d => d.DevicesID == id);
            // Envia o dispositivo para a View para o utilizador confirmar qual item será apagado
            return View(dispositivo);
        }
        

        // 1. CAMPO PRIVADO PARA GUARDAR A INSTÂNCIA DO REPOSITÓRIO
        private readonly InterfaceRepositoryMDev _repository;

        // 2. CONSTRUTOR: O ASP.NET CORE INJETA A CLASSE CORRETA (EFRepositoryMedicalDevices) AQUI
        public MedicalDevicesController(InterfaceRepositoryMDev repository)
        {
            _repository = repository;
        }
        public IActionResult Index() //Controller da lista dos dispositivos móveis
        {
            var listaDeDispositivos = _repository.devices;

            //Mostra a lista de dispositivos -> Index.cshtml.
            return View(listaDeDispositivos);
        }

        //----------------------------------------------------Processa a submissão(POST)
        //1-> CreateMedicalDevices
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMedicalDevices(MedicalDevices device)
        {
            if (ModelState.IsValid)
            {
                // CORRIGIDO: Chama o método Add no Repositório Real
                _repository.AddMedicalDevices(device); // Se a sua interface tiver Add!

                TempData["Mensagem de sucesso"] = $"Dispositivo '{device.Name}' Registado com sucesso!";

                // Redirecionar para o Index é melhor para ver a lista atualizada
                return RedirectToAction("Index");
            }

            //Se a validação falhar, mostra a View CreateMedicalDevices 
            return View(device);
        }


        //2-> ReadMedicalDevices
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateMedicalDevices(MedicalDevices device)
        {
            // Verifica as regras de validação
            if (ModelState.IsValid)
            {
                //Chama o método do Repositório para atualizar o item na lista
                _repository.UpdateMedicalDevices(device);
                //Redireciona para a lista de dispositivos móveis
                return RedirectToAction("Index");
            }

            //Se a validação falhar, mostra a View UpdateMedicalDevices
            return View(device);
        }


        //3-> DeleteMedicalDevices
        [HttpPost]
        [ActionName("DeleteMedicalDevices")] //liga a ação
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            //Chama o método do Repositório para remover o item
            _repository.DeleteMedicalDevices(id);

            // Redireciona de volta para a lista após a eliminação
            return RedirectToAction("Index");
        }
    }
}
