using System;

namespace WingsOn.Api.Models
{
	public class PersonViewModel
	{
		public string Name { get; set; }

		public DateTime DateBirth { get; set; }

		public GenderTypeViewModel Gender { get; set; }

		public string Address { get; set; }

		public string Email { get; set; }

		public int Id { get; set; }
	}
}
