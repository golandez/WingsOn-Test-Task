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
	public class FlightServiceTest
	{
		private FlightService _service;

		public FlightServiceTest()
		{
			var _mockFlights = new Mock<IRepository<Flight>>();

			_service = new FlightService(_mockFlights.Object);

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

			_mockFlights.Setup(x => x.GetAll()).Returns(listOfFlight);
		}


		[Fact]
		public void IsFlightExistReturnsTrue()
		{
			// Arrange
			var flightNumber = "TS001";

			// Act
			var result = _service.IsFlightExist(flightNumber);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsFlightExistReturnsFalse()
		{
			// Arrange
			var flightNumber = "TS123";

			// Act
			var result = _service.IsFlightExist(flightNumber);

			// Assert
			Assert.False(result);
		}
	}
}
