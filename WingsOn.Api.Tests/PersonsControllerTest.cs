using WingsOn.Api.Controllers;
using Xunit;
using Moq;
using AutoMapper;
using WingsOn.Servces;
using Microsoft.Extensions.Logging;
using WingsOn.Api.Models;
using Microsoft.AspNetCore.Mvc;
using WingsOn.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace WingsOn.Api.Tests
{
	public class PersonsControllerTest
	{
		private PersonsController _controller;
		private Mock<ILogger<PersonsController>> _mockLogger;
		private Mock<IPersonService> _mockPersonService;

		public PersonsControllerTest()
		{
			_mockLogger = new Mock<ILogger<PersonsController>>();

			//auto mapper configuration
			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new MappingProfile());
			});
			var mapper = mapperConfig.CreateMapper();

			_mockPersonService = new Mock<IPersonService>();

			_controller = new PersonsController(_mockPersonService.Object, mapper, _mockLogger.Object);
		}

		[Fact]
		public void GetReturnsBadRequest()
		{
			// Arrange
			var testPersonId = 0;

			// Act
			var result = _controller.Get(testPersonId);

			// Assert
			var objectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("Incorrect passanger's id", objectResult.Value);
		}

		[Fact]
		public void GetReturnsPersonViewModel()
		{
			// Arrange
			var testPersonId = 5;
			var testPerson = new Person
			{
				Id = testPersonId,
				Name = "Test name"
			};
			_mockPersonService.Setup(x => x.Get(testPersonId)).Returns(testPerson);

			// Act
			var result = _controller.Get(testPersonId);

			// Assert
			var model = Assert.IsType<PersonViewModel>(result.Value);

			Assert.Equal(testPerson.Name, model.Name);
		}

		[Fact]
		public void GetReturnsNotFound()
		{
			// Arrange
			var testPersonId = 5;
			_mockPersonService.Setup(x => x.Get(testPersonId)).Returns((Person)null);

			// Act
			var result = _controller.Get(testPersonId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public void PatchReturnsBadRequest()
		{
			// Arrange
			var testPersonId = 0;

			// Act
			var result = _controller.Patch(testPersonId, null);

			// Assert
			var objectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Incorrect passanger's id", objectResult.Value);
		}

		[Fact]
		public void PatchReturnsNoContent()
		{
			// Arrange
			var testPersonId = 5;
			var testPerson = new Person();
			_mockPersonService.Setup(x => x.Get(testPersonId)).Returns(testPerson);
			var patch = new JsonPatchDocument<PersonViewModel>();
			patch.Operations.Add(new Operation<PersonViewModel>("replace","/Address", null, "New address"));

			// Act
			var result = _controller.Patch(testPersonId, patch);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public void PatchReturnsNotFound()
		{
			// Arrange
			var testPersonId = 5;
			_mockPersonService.Setup(x => x.Get(testPersonId)).Returns((Person)null);

			// Act
			var result = _controller.Patch(testPersonId, null);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}


	}
}
