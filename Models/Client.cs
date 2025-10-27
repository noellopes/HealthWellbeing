using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
	public class Client
	{
		public string ClientId { get; set; } = string.Empty;

		[Required(ErrorMessage = "First and last name are necessary.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "The name must have at least 6 chars and no more than 100.")]
		[RegularExpression(@"^\p{L}+(\s\p{L}+)+$", ErrorMessage = "Introduce at least a First and Last name!")]
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

		public bool? CreateMember { get; set; }

		public string CreateMemberText => CreateMember switch
		{
			true => "Yes",
			false => "No",
			_ => "Pending Acceptation..",
		};

        public ICollection<Client>? Clients { get; set; } = default;
    }
}

