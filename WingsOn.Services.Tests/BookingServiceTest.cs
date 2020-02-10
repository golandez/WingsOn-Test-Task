using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WingsOn.Dal;
using WingsOn.Domain;
using WingsOn.Servces;
using Xunit;

namespace WingsOn.Services.Tests
{
	public class BookingServiceTest
	{
		private BookingService _service;

		public BookingServiceTest()
		{
			var _mockBookings = new Mock<IRepository<Booking>>();
			var _mockPersons = new Mock<IRepository<Person>>();
			var _mockFlights = new Mock<IRepository<Flight>>();

			_service = new BookingService(_mockBookings.Object, _mockPersons.Object, _mockFlights.Object);

			var listOfPerson = new List<Person> {
				new Person
				{
					Id = 1,
					Address = "address1",
					Email = "1@mail.com",
					Gender = GenderType.Male,
					Name = "Male 1"
				},
				new Person
				{
					Id = 2,
					Address = "address2",
					Email = "2@mail.com",
					Gender = GenderType.Female,
					Name = "Female 2"
				},
				new Person
				{
					Id = 3,
					Address = "address3",
					Email = "3@mail.com",
					Gender = GenderType.Male,
					Name = "Male 3"
				}};

			var listOfFlight = new List<Flight> {
			new Flight
				{
					Id = 1,
					Number = "TS001"
				},
				new Flight
				{
					Id = 1,
					Number = "TS002"
				}};

			var listOfBooking = new List<Booking>
			{
				new Booking
				{
					Id = 1,
					Number = "WO-000001",
					Customer = listOfPerson.First(),
					Flight = listOfFlight.First(),
					Passengers = new []
					{
						listOfPerson.First()
					}
				},
				new Booking
				{
					Id = 2,
					Number = "WO-000002",
					Customer = listOfPerson.Skip(1).First(),
					Flight = listOfFlight.Skip(1).First(),
					Passengers = listOfPerson.Skip(1).Take(2)
				}
			};
			_mockPersons.Setup(x => x.GetAll()).Returns(listOfPerson);
			_mockFlights.Setup(x => x.GetAll()).Returns(listOfFlight);
			_mockBookings.Setup(x => x.GetAll()).Returns(listOfBooking);
		}

		[Fact]
		public void GetAllPassengersByGenderReturnsMaleList()
		{
			// Arrange

			// Act
			var result = _service.GetAllPassengersByGender(GenderType.Male);

			// Assert
			Assert.Equal(2, result.Count());
		}

		[Fact]
		public void GetAllPassengersByGenderReturnsFemaleList()
		{
			// Arrange

			// Act
			var result = _service.GetAllPassengersByGender(GenderType.Female);

			// Assert
			Assert.Single(result);
		}

		[Fact]
		public void GetPassengersByFlightReturnsList()
		{
			// Arrange
			var flightNumber = "TS002";

			// Act
			var result = _service.GetPassengersByFlight(flightNumber);

			// Assert
			Assert.Equal(2, result.Count());
		}

		[Fact]
		public void GetPassengersByFlightReturnsEmptyList()
		{
			// Arrange
			var flightNumber = "TS123";

			// Act
			var result = _service.GetPassengersByFlight(flightNumber);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void IsPassangerBookedReturnsTrue()
		{
			// Arrange
			var flightNumber = "TS001";
			var passangerName = "Male 1";

			// Act
			var result = _service.IsFlightBooked(flightNumber, passangerName);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsPassangerBookedReturnsFalse()
		{
			// Arrange
			var flightNumber = "TS001";
			var passangerName = "Female 2";

			// Act
			var result = _service.IsFlightBooked(flightNumber, passangerName);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task BookPassangerAsyncReturnsNewBooking()
		{
			// Arrange
			var flightNumber = "TS001";
			var passangerName = "Female 2";

			// Act
			var result = await _service.BookFlightAsync(passangerName, flightNumber);

			// Assert
			Assert.Equal(3, result.Id);
			Assert.Equal(passangerName, result.Customer.Name);
		}
	}
}
