using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WingsOn.Api.Models;
using WingsOn.Servces;

namespace WingsOn.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FlightsController : ControllerBase
	{

		private readonly IBookingService _bookings;
		private readonly IFlightService _flights;
		private readonly IMapper _mapper;
		private readonly ILogger<FlightsController> _logger;

		public FlightsController(IFlightService flightService, IBookingService bookingService, IMapper mapper, ILogger<FlightsController> logger)
		{
			_flights = flightService;
			_bookings = bookingService;
			_mapper = mapper;
			_logger = logger;
		}

		// GET: api/Flights/{number}/Passengers
		[HttpGet("{number}/Passengers", Name = "GetPassengersByFlight")]
		public ActionResult<IList<PersonViewModel>> GetPassengersByFlight(string number)
		{
			try
			{
				if(!_flights.IsFlightExist(number))
					return BadRequest("Incorrect flight number");

				var passengers = _bookings.GetPassengersByFlight(number);

				if (passengers.Any())
				{
					var result = _mapper.Map<IEnumerable<PersonViewModel>>(passengers);
					return result.ToList();
				}
				return NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError("Error during the receving the passangers", ex);
				throw;
			}
		}
	}
}
