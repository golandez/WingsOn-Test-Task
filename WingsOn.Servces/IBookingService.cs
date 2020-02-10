using System.Collections.Generic;
using System.Threading.Tasks;
using WingsOn.Domain;

namespace WingsOn.Servces
{
	public interface IBookingService
	{
		IEnumerable<Person> GetAllPassengersByGender(GenderType genderType);
		IEnumerable<Person> GetPassengersByFlight(string number);
		bool IsFlightBooked(string flightNumber, string passangersName);
		Task<Booking> BookFlightAsync(string passangersName, string flightNumber);
	}
}
