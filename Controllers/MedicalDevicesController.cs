using HealthWellbeingRoom.Models.FileMobileDevices;
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
            var dispositivo = RepositoryMedicalDevices.Index.FirstOrDefault(d => d.DevicesID == id);

            return View(dispositivo);
        }

        public IActionResult UpdateMedicalDevices(int id)
        {
            var dispositivo = RepositoryMedicalDevices.Index.FirstOrDefault(d => d.DevicesID == id);

            // Envia o registo encontrado para a View para pré-preenchimento
            return View(dispositivo);
        }

        public IActionResult DeleteMedicalDevices(int id)
        {
            var dispositivo = RepositoryMedicalDevices.Index.FirstOrDefault(d => d.DevicesID == id);

            // Envia o dispositivo para a View para o utilizador confirmar qual item será apagado
            return View(dispositivo);
        }
        
        public IActionResult Index() //Controller da lista dos dispositivos móveis
        {
            //Pega a lista de dispositivos no Repositório.
            var listaDeDispositivos = RepositoryMedicalDevices.Index;

            //Mostra a lista de dispositivos -> Index.cshtml.
            return View(listaDeDispositivos);
        }

        //----------------------------------------------------Processa a submissão(POST)
        //1-> CreateMedicalDevices
        [HttpPost]
        public IActionResult CreateMedicalDevices(MedicalDevices dispositivo)
        {
            if (ModelState.IsValid)
            {
                //Obtém a lista de dispositivos móveis
                var listaAtual = RepositoryMedicalDevices.Index;

                //Gera novo id sequencial começando de 1
                dispositivo.DevicesID = listaAtual.Count() + 1;

                //Regista data e hora da criação
                dispositivo.DataRegisto = DateTime.Now;

                //Guarda dados
                RepositoryMedicalDevices.AddMedicalDevices(dispositivo);

                TempData["Mensagem de sucesso"] = $"Dispositivo '{dispositivo.NomeDisp}' Registado com sucesso!";

                //Retorna a view CreateMedicalDevices com todos os campos vazios
                return View(new MedicalDevices());
            }

            //Se a validação falhar, mostra a View CreateMedicalDevices 
            return View(dispositivo);
        }


        //2-> ReadMedicalDevices
        [HttpPost]
        public IActionResult UpdateMedicalDevices(MedicalDevices dispositivo)
        {
            // Verifica as regras de validação
            if (ModelState.IsValid)
            {
                //Chama o método do Repositório para atualizar o item na lista
                RepositoryMedicalDevices.UpdateMedicalDevices(dispositivo);

                //Redireciona para a lista de dispositivos móveis
                return RedirectToAction("Index");
            }

            //Se a validação falhar, mostra a View UpdateMedicalDevices
            return View(dispositivo);
        }


        //3-> DeleteMedicalDevices
        [HttpPost]
        [ActionName("DeleteMedicalDevices")] //liga a ação
        public IActionResult DeleteConfirmed(int id)
        {
            //Chama o método do Repositório para remover o item
            RepositoryMedicalDevices.DeleteMedicalDevices(id);

            // Redireciona de volta para a lista após a eliminação
            return RedirectToAction("Index");
        }
    }
}
