using System;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WingsOn.Api.Models;
using WingsOn.Servces;

namespace WingsOn.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PersonsController : ControllerBase
	{
		private readonly IPersonService _persons;
		private readonly IMapper _mapper;
		private readonly ILogger<PersonsController> _logger;

		public PersonsController(IPersonService personService, IMapper mapper, ILogger<PersonsController> logger)
		{
			_persons = personService;
			_mapper = mapper;
			_logger = logger;
		}

		// GET: api/Persons/5
		[HttpGet("{id}", Name = "Get")]
		public ActionResult<PersonViewModel> Get(int id)
		{
			try
			{
				if(id < 1)
					return BadRequest("Incorrect passanger's id");

				var person = _persons.Get(id);

				if (person == null)
				{
					return NotFound();
				}

				var personViewModel = _mapper.Map<PersonViewModel>(person);

				return personViewModel;
			}
			catch (Exception ex)
			{
				_logger.LogError("Error during the receiving  of person", ex);
				throw;
			}
		}

		// PATCH: api/Persons/5
		[HttpPatch("{id}")]
		public IActionResult Patch(int id, [FromBody] JsonPatchDocument<PersonViewModel> pacthDoc)
		{
			try
			{
				if (id < 1)
					return BadRequest("Incorrect passanger's id");

				var person = _persons.Get(id);

				if (person != null)
				{
					var personViewModel = _mapper.Map<PersonViewModel>(person);
					pacthDoc.ApplyTo(personViewModel);

					_mapper.Map(personViewModel, person);

					_persons.Save(person);

					return NoContent();
				}
				return NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError("Error during the updating of the person's address", ex);
				throw;
			}
		}
	}
}
