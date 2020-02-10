using System.ComponentModel.DataAnnotations;

namespace WingsOn.Api.Models
{
	public class NewBookingViewModel
	{
		[Required]
		[MinLength(5)]
		public string FlightNumber { get; set; }

		[Required]
		public string PassengersName { get; set; }
	} 
}
