using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Models
{
	public class Client
	{
		public string ClientId { get; set; }

		[Required(ErrorMessage = "First and last name are necessary.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "The name must have at least 6 chars and no more than 100.")]
		[RegularExpression(@"^[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+(?:\s[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+)+$", ErrorMessage = "Please introduce at least a First and Last name!")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email address is required.")]
		[EmailAddress(ErrorMessage = "Enter a valid email address.")]
		[StringLength(100)]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "A phone number is required.")]
		[Phone(ErrorMessage = "Enter a valid phone number.")]
		[StringLength(15)]
		public string Phone { get; set; } = string.Empty;

		[StringLength(200)]
		public string Address { get; set; } = string.Empty;

		[DataType(DataType.Date)]
		public DateTime? BirthDate { get; set; }

		[StringLength(10)]
		public string Gender { get; set; } = string.Empty;

		[DataType(DataType.Date)]
		public DateTime RegistrationDate { get; set; } = DateTime.Now;
		public Member? Membership { get; set; }
		
		[StringLength(450)]
		public string? IdentityUserId { get; set; }

		public IdentityUser? IdentityUser { get; set; }
		public int PlanoAlimentarId { get; set; }

		public PlanoAlimentar? PlanoAlimentar { get; set; }

		public ICollection<ClientAlergia>? Alergias { get; set; }
		public ICollection<ClientRestricao>? RestricoesAlimentares { get; set; }


	}
}

