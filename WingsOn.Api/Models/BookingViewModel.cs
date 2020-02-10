using System;
using System.Collections.Generic;

namespace WingsOn.Api.Models
{
	public class BookingViewModel
	{
		public string Number { get; set; }

		public string FlightNumber { get; set; }

		public PersonViewModel Customer { get; set; }

		public IEnumerable<PersonViewModel> Passengers { get; set; }

		public DateTime DateBooking { get; set; }

		public int Id { get; set; }
	}
}
