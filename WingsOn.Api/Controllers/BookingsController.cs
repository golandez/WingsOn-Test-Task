using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WingsOn.Api.Models;
using WingsOn.Domain;
using WingsOn.Servces;

namespace WingsOn.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingsController : ControllerBase
	{
		private readonly IBookingService _bookings;
		private readonly IFlightService _flights;
		private readonly IMapper _mapper;
		private readonly ILogger<BookingsController> _logger;

		public BookingsController(IBookingService bookingService, IFlightService flightService, IMapper mapper, ILogger<BookingsController> logger)
		{
			_bookings = bookingService;
			_flights = flightService;
			_mapper = mapper;
			_logger = logger;
		}

		// GET: api/Bookings/Passengers?gender={gender}
		[HttpGet("Passengers", Name = "GetAllPassengersByGender")]
		public ActionResult<IList<PersonViewModel>> GetAllPassengersByGender([FromQuery] GenderTypeViewModel gender)
		{
			try
			{
				var genderType = _mapper.Map<GenderType>(gender);
				var passengers = _bookings.GetAllPassengersByGender(genderType);

				if (passengers.Any())
				{
					var result = _mapper.Map<IEnumerable<PersonViewModel>>(passengers);
					return result.ToList();
				}
				return NotFound();
			}
			catch(Exception ex)
			{
				_logger.LogError("Error during the receving the passangers", ex);
				throw;
			}
		}

		// POST: api/Booking
		[HttpPost]
		public async Task<IActionResult> PostAsync([FromBody] NewBookingViewModel newBooking)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState.SelectMany(x => x.Value.Errors));

				if (!_flights.IsFlightExist(newBooking.FlightNumber))
					return BadRequest("Incorrect flight number");

				if (_bookings.IsFlightBooked(newBooking.FlightNumber, newBooking.PassengersName))
					return BadRequest("This passanger has already booked on this flight");

				var createdBooking = await _bookings.BookFlightAsync(newBooking.PassengersName, newBooking.FlightNumber);

				var result = _mapper.Map<BookingViewModel>(createdBooking);

				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError("Error during the booking the passanger", ex);
				throw;
			}
		}
	}
}
