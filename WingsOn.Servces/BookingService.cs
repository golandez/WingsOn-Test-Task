using System.Collections.Generic;
using System.Linq;
using WingsOn.Dal;
using WingsOn.Domain;
using System;
using System.Threading.Tasks;

namespace WingsOn.Servces
{
	public class BookingService : IBookingService
	{
		private IRepository<Booking> _bookings;
		private IRepository<Person> _persons;
		private IRepository<Flight> _flights;

		public BookingService(IRepository<Booking> bookingRepository, IRepository<Person> personRepository, IRepository<Flight> flightRepository)
		{
			_bookings = bookingRepository;
			_persons = personRepository;
			_flights = flightRepository;
		}

		public IEnumerable<Person> GetAllPassengersByGender(GenderType genderType)
		{
			var passengers = _bookings.GetAll().SelectMany(x => x.Passengers).Where(y => y.Gender == genderType);
			return passengers;
		}

		public IEnumerable<Person> GetPassengersByFlight(string number)
		{
			var passengers = _bookings.GetAll().Where(x => x.Flight.Number.Equals(number)).SelectMany(y => y.Passengers);
			return passengers;
		}

		public bool IsFlightBooked(string flightNumber, string passangersName)
		{
			return _bookings.GetAll().Any(x => x.Flight.Number.Equals(flightNumber) && x.Passengers.Any(y => y.Name.Equals(passangersName)));
		}
		public async Task<Booking> BookFlightAsync(string passangersName, string flightNumber)
		{
			var newPassanger = _persons.GetAll().SingleOrDefault(p => p.Name.Equals(passangersName));

			if( newPassanger == null)
			{
				newPassanger = new Person { Name = passangersName };
				_persons.Save(newPassanger);
			}

			var newBooking = new Booking
			{
				Id = GenerateNewBookingId(),
				Number = await GenerateNewBookingNumberAsync(),
				Customer = newPassanger,
				DateBooking = DateTime.UtcNow,
				Flight = _flights.GetAll().Single(f => f.Number.Equals(flightNumber)),
				Passengers = new[] { newPassanger }
			};

			_bookings.Save(newBooking);
			return newBooking;
		}

		private int GenerateNewBookingId()
		{
			return _bookings.GetAll().Max(x => x.Id) + 1;
		}

		private async Task<string> GenerateNewBookingNumberAsync()
		{
			var generator = new Random();
			return await Task.FromResult<string>($"WO-{generator.Next(0, 999999).ToString("D6")}");
		}
	}
}
