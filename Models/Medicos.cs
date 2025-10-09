using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
	public class Medicos : Controller
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "O nome do médico é obrigatório")]
		[StringLength(100)]
		public string Nome { get; set; }

		public string Especialidade {  get; set; }
		public string Telefone { get; set; }
		public string Email { get; set; }


	}
}
