using WingsOn.Api.Controllers;
using Xunit;
using Moq;
using AutoMapper;
using WingsOn.Servces;
using Microsoft.Extensions.Logging;
using WingsOn.Api.Models;
using Microsoft.AspNetCore.Mvc;
using WingsOn.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WingsOn.Api.Tests
{
	public class FlightsControllerTest
	{
		private FlightsController _controller;
		private Mock<ILogger<FlightsController>> _mockLogger;
		private Mock<IBookingService> _mockBookingService;
		private Mock<IFlightService> _mockFlightService;

		public FlightsControllerTest()
		{
			_mockLogger = new Mock<ILogger<FlightsController>>();

			//auto mapper configuration
			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new MappingProfile());
			});
			var mapper = mapperConfig.CreateMapper();

			_mockBookingService = new Mock<IBookingService>();
			_mockFlightService = new Mock<IFlightService>();

			_controller = new FlightsController(_mockFlightService.Object, _mockBookingService.Object, mapper, _mockLogger.Object);
		}

		[Fact]
		public void GetPassengersByFlightReturnsBadRequest()
		{
			// Arrange
			var testFlightNumber = "TS123";

			_mockFlightService.Setup(x => x.IsFlightExist(testFlightNumber)).Returns(false);

			// Act
			var result = _controller.GetPassengersByFlight(testFlightNumber);

			// Assert
			var objectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("Incorrect flight number", objectResult.Value);
		}

		[Fact]
		public void GetPassengersByFlightReturnsNotFound()
		{
			// Arrange
			var testFlightNumber = "TS123";
			_mockFlightService.Setup(x => x.IsFlightExist(testFlightNumber)).Returns(true);
			_mockBookingService.Setup(x => x.GetPassengersByFlight(testFlightNumber)).Returns(new List<Person>());

			// Act
			var result = _controller.GetPassengersByFlight(testFlightNumber);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public void GetPassengersByFlightReturnsListOfPersons()
		{
			// Arrange
			var testFlightNumber = "TS123";
			_mockFlightService.Setup(x => x.IsFlightExist(testFlightNumber)).Returns(true);

			var listOfPersons = new List<Person>
			{
				new Person
				{
					Name = "Peson 1",
					Gender = GenderType.Female
				},
				new Person
				{
					Name = "Peson 2",
					Gender = GenderType.Male
				}
			};

			_mockBookingService.Setup(x => x.GetPassengersByFlight(testFlightNumber)).Returns(listOfPersons);

			// Act
			var result = _controller.GetPassengersByFlight(testFlightNumber);

			// Assert
			Assert.IsAssignableFrom<IList<PersonViewModel>>(result.Value);
			Assert.Equal(2, result.Value.Count);
		}

	}
}
