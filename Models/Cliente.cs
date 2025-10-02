using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Cliente
    {
		[Required(ErrorMessage = "First and last name are necessary.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "The name must have at least 6 chars and no more than 100")]
		[RegularExpression(@"^[A-Za-zÀ-ÿ]+( [A-Za-zÀ-ÿ]+)+$", ErrorMessage = "Introduce at least a First and Last name!")]
		public string Name { get; set; }

		[Required(ErrorMessage = "A Google mail address is required (@gmail)")]
		[EmailAddress(ErrorMessage = "Enter a valid gmail address")]
		[RegularExpression(@"^[a-zA-Z0-9._-]+@gmail\.com$", ErrorMessage = "Email must contain @gmail.com")]
		public string Email { get; set; }

		[Required(ErrorMessage = "A Portuguese Phone Number is required")]
		[Phone(ErrorMessage = "Enter a valid Portuguese Phone Number")]
		[RegularExpression(@"^9[1236]\d{7}$", ErrorMessage = "The number has to start with 91, 92, 93 or 96...")]
		public string Phone { get; set; }

		public bool? IsMember { get; set; }

		public string IsMemberText => IsMember switch
		{
			true => "Yes",
			false => "No",
		};
	}
}
