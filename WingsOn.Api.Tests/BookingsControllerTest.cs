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
	public class BookingsControllerTest
	{
		private BookingsController _controller;
		private Mock<ILogger<BookingsController>> _mockLogger;
		private Mock<IBookingService> _mockBookingService;
		private Mock<IFlightService> _mockFlightService;

		public BookingsControllerTest()
		{
			_mockLogger = new Mock<ILogger<BookingsController>>();

			//auto mapper configuration
			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new MappingProfile());
			});
			var mapper = mapperConfig.CreateMapper();

			_mockBookingService = new Mock<IBookingService>();
			_mockFlightService = new Mock<IFlightService>();

			_controller = new BookingsController(_mockBookingService.Object, _mockFlightService.Object, mapper, _mockLogger.Object);
		}

		//[Fact]
		//public void GetPassengersByFlightReturnsNotFound()
		//{
		//	// Arrange
		//	var testFlightNumber = "TS123";

		//	_mockBookingService.Setup(x => x.GetPassengersByFlight(testFlightNumber)).Returns(new List<Person>());

		//	// Act
		//	var result = _controller.GetPassengersByFlight(testFlightNumber);

		//	// Assert
		//	Assert.IsType<NotFoundResult>(result.Result);
		//}

		//[Fact]
		//public void GetPassengersByFlightReturnsListOfPersons()
		//{
		//	// Arrange
		//	var testFlightNumber = "TS123";
		//	var listOfPersons = new List<Person>
		//	{
		//		new Person
		//		{
		//			Name = "Peson 1",
		//			Gender = GenderType.Female
		//		},
		//		new Person
		//		{
		//			Name = "Peson 2",
		//			Gender = GenderType.Male
		//		}
		//	};

		//	_mockBookingService.Setup(x => x.GetPassengersByFlight(testFlightNumber)).Returns(listOfPersons);

		//	// Act
		//	var result = _controller.GetPassengersByFlight(testFlightNumber);

		//	// Assert
		//	Assert.IsAssignableFrom<IList<PersonViewModel>>(result.Value);
		//	Assert.Equal(2, result.Value.Count);
		//}

		[Fact]
		public void GetAllPassengersByGenderReturnsNotFound()
		{
			// Arrange
			_mockBookingService.Setup(x => x.GetAllPassengersByGender(It.IsAny<GenderType>())).Returns(new List<Person>());

			// Act
			var result = _controller.GetAllPassengersByGender(GenderTypeViewModel.Male);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public void GetAllPassengersByGenderReturnsListOfPersons()
		{
			// Arrange
			var listOfPersons = new List<Person>
			{
				new Person
				{
					Name = "Peson 2",
					Gender = GenderType.Male
				}
			};

			_mockBookingService.Setup(x => x.GetAllPassengersByGender(GenderType.Male)).Returns(listOfPersons);

			// Act
			var result = _controller.GetAllPassengersByGender(GenderTypeViewModel.Male);

			// Assert
			Assert.IsAssignableFrom<IList<PersonViewModel>>(result.Value);
			Assert.Equal(1, result.Value.Count);
		}


		[Fact]
		public async Task PostAsyncWithValidationErrorsReturnsBadRequest()
		{
			// Arrange
			var newBooking = new NewBookingViewModel()
			{
				PassengersName = "PersonName"
			};
			_controller.ModelState.AddModelError("FlightNumber", "The FlightNumber field is required.");

			// Act
			var result = await _controller.PostAsync(newBooking);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			
		}

		[Fact]
		public async Task PostAsyncReturnsBadRequest()
		{
			// Arrange
			var newBooking = new NewBookingViewModel()
			{
				FlightNumber = "TS123",
				PassengersName = "PersonName"
			};
			_mockFlightService.Setup(x => x.IsFlightExist(It.IsAny<string>())).Returns(true);
			_mockBookingService.Setup(x => x.IsFlightBooked(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			// Act
			var result = await _controller.PostAsync(newBooking);

			// Assert
			var objectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("This passanger has already booked on this flight", objectResult.Value);
		}

		[Fact]
		public async Task PostAsyncReturnsNewBooking()
		{
			// Arrange
			var newBooking = new NewBookingViewModel()
			{
				FlightNumber = "TS123",
				PassengersName = "Person Name"
			};
			var booking = new Booking
			{
				Id = 1,
				Number = "WO-123456",
				Flight = new Flight
				{
					Number = "TS123"
				},
				Customer = new Person
				{
					Name = "Person Name"
				}
			};
			_mockFlightService.Setup(x => x.IsFlightExist(newBooking.FlightNumber)).Returns(true);
			_mockBookingService.Setup(x => x.IsFlightBooked(newBooking.FlightNumber, newBooking.PassengersName)).Returns(false);
			_mockBookingService.Setup(x => x.BookFlightAsync(newBooking.PassengersName, newBooking.FlightNumber)).Returns(Task.FromResult(booking));

			// Act
			var result = await _controller.PostAsync(newBooking);

			// Assert
			var objectResult = Assert.IsType<OkObjectResult>(result);

			var viewModel = Assert.IsType<BookingViewModel>(objectResult.Value);
			Assert.Equal(newBooking.FlightNumber, viewModel.FlightNumber);
			Assert.Equal(newBooking.PassengersName, viewModel.Customer.Name);
		}
	}
}
