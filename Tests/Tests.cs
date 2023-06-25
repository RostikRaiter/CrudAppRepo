using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DataLayer.Models;
using DataLayer.Models.Domain;
using MVCCRUD.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Tests
{
	public class EmployeesControllerTests
	{
		private readonly CrudDbContext _context;
		private readonly EmployeesController _controller;

		public EmployeesControllerTests()
		{
			var options = new DbContextOptionsBuilder<CrudDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new CrudDbContext(options);
			_context.Employees.Add(new Employee { Id = Guid.NewGuid(), Name = "Test", Email = "test@test.com", Salary = 50000, DateOfBirth = DateTime.Now, Department = "IT" });
			_context.SaveChanges();

			_controller = new EmployeesController(_context);
		}

		[Fact]
		public async Task Index_ReturnsAViewResult_WithAListOfEmployees()
		{
			// Act
			var result = await _controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<List<Employee>>(viewResult.ViewData.Model);
			Assert.Single(model);
		}

		[Fact]
		public async Task Add_ReturnsARedirectToActionResult_WhenModelIsValid()
		{
			// Arrange
			var newEmployee = new AddEmployeeViewModel
			{
				Name = "Test",
				Email = "test@test.com",
				Salary = 50000,
				DateOfBirth = DateTime.Now,
				Department = "IT"
			};

			// Act
			var result = await _controller.Add(newEmployee);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

		[Fact]
		public async Task View_ReturnsAViewResult_WithEmployeeModel()
		{
			// Arrange
			var employeeId = _context.Employees.First().Id;

			// Act
			var result = await _controller.View(employeeId);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<UpdateEmployeeViewModel>(viewResult.ViewData.Model);
			Assert.Equal("Test", model.Name);
			Assert.Equal("test@test.com", model.Email);
			Assert.Equal(50000, model.Salary);
			Assert.Equal("IT", model.Department);
		}

		[Fact]
		public async Task Delete_ReturnsARedirectToActionResult_WhenEmployeeExists()
		{
			// Arrange
			var employeeId = _context.Employees.First().Id;
			var employeeViewModel = new UpdateEmployeeViewModel
			{
				Id = employeeId,
				Name = "Test",
				Email = "test@test.com",
				Salary = 50000,
				DateOfBirth = DateTime.Now,
				Department = "IT"
			};

			// Act
			var result = await _controller.Delete(employeeViewModel);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}


		[Fact]
		public async Task Update_ReturnsARedirectToActionResult_WhenModelIsValid()
		{
			// Arrange
			var employeeId = _context.Employees.First().Id;
			var employee = new UpdateEmployeeViewModel
			{
				Id = employeeId,
				Name = "Test Updated",
				Email = "testupdated@test.com",
				Salary = 60000,
				DateOfBirth = DateTime.Now,
				Department = "HR"
			};

			// Act
			var result = await _controller.View(employee);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}
	}
}
