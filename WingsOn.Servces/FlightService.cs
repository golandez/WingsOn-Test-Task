using System.Linq;
using WingsOn.Dal;
using WingsOn.Domain;

namespace WingsOn.Servces
{
	public class FlightService : IFlightService
	{
		private IRepository<Flight> _flights;

		public FlightService(IRepository<Flight> flightRepository)
		{
			_flights = flightRepository;
		}

		public bool IsFlightExist(string number)
		{
			return _flights.GetAll().Any(x => x.Number.Equals(number));
		}
	}
}
