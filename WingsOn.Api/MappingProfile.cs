using AutoMapper;
using WingsOn.Api.Models;
using WingsOn.Domain;

namespace WingsOn.Api
{
	public class MappingProfile: Profile
	{
		public MappingProfile()
		{
			CreateMap<Person, PersonViewModel>();
			CreateMap<PersonViewModel, Person>();
			CreateMap<GenderType, GenderTypeViewModel>();
			CreateMap<Booking, BookingViewModel>().AfterMap((s, d) => d.FlightNumber = s.Flight.Number);
		}
	}
}
