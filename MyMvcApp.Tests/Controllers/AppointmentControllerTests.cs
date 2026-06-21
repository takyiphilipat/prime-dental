using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Controllers;
using MyMvcApp.Data;
using MyMvcApp.Models;
using MyMvcApp.Models.Requests;
using Xunit;

namespace MyMvcApp.Tests.Controllers
{
    public class AppointmentControllerTests
    {
        // Minimal fake so TempData works without a real HTTP request pipeline
        private class FakeTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }

        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private static AppointmentController CreateControllerWithTempData(ApplicationDbContext context)
        {
            return new AppointmentController(context)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider())
            };
        }

        [Fact]
        public async Task Create_ValidRequest_SavesAppointment_AndRedirects()
        {
            var context = CreateInMemoryContext();
            var controller = CreateControllerWithTempData(context);

            var request = new AppointmentRequest
            {
                Name = "Philip",
                Email = "philip@example.com",
                Service = "Dental Cleaning",
                Gender = "Male",
                AppointmentDate = DateTime.Now.AddDays(3)
            };

            var result = await controller.Create(request);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, await context.Appointments.CountAsync());

            var saved = await context.Appointments.FirstAsync();
            Assert.Equal("Philip", saved.Name);
            Assert.Equal("Male", saved.Gender);
        }

        [Fact]
        public async Task Create_InvalidModelState_DoesNotSave_AndRedirects()
        {
            var context = CreateInMemoryContext();
            var controller = CreateControllerWithTempData(context);

            // Simulate what the model binder would do if validation failed
            controller.ModelState.AddModelError("Name", "Please enter your name");

            var request = new AppointmentRequest
            {
                Name = "",
                Email = "philip@example.com",
                Service = "Dental Cleaning",
                Gender = "Male",
                AppointmentDate = DateTime.Now.AddDays(3)
            };

            var result = await controller.Create(request);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(0, await context.Appointments.CountAsync());
        }

        [Fact]
        public async Task Create_DuplicateBooking_SameEmailAndDate_IsBlocked()
        {
            var context = CreateInMemoryContext();

            var existingDate = DateTime.Now.AddDays(5);
            context.Appointments.Add(new Appointment
            {
                Name = "Philip",
                Email = "philip@example.com",
                Service = "Dental Cleaning",
                Gender = "Male",
                AppointmentDate = existingDate
            });
            await context.SaveChangesAsync();

            var controller = CreateControllerWithTempData(context);

            var duplicateRequest = new AppointmentRequest
            {
                Name = "Philip",
                Email = "philip@example.com",
                Service = "Orthodontics",
                Gender = "Male",
                AppointmentDate = existingDate
            };

            var result = await controller.Create(duplicateRequest);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, await context.Appointments.CountAsync());
        }

        [Fact]
        public async Task Create_SameEmail_DifferentDate_IsAllowed()
        {
            var context = CreateInMemoryContext();

            context.Appointments.Add(new Appointment
            {
                Name = "Philip",
                Email = "philip@example.com",
                Service = "Dental Cleaning",
                Gender = "Male",
                AppointmentDate = DateTime.Now.AddDays(5)
            });
            await context.SaveChangesAsync();

            var controller = CreateControllerWithTempData(context);

            var newRequest = new AppointmentRequest
            {
                Name = "Philip",
                Email = "philip@example.com",
                Service = "Orthodontics",
                Gender = "Male",
                AppointmentDate = DateTime.Now.AddDays(10)
            };

            var result = await controller.Create(newRequest);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(2, await context.Appointments.CountAsync());
        }
    }
}
