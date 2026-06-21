using System.ComponentModel.DataAnnotations;
using MyMvcApp.Models.Requests;
using Xunit;

namespace MyMvcApp.Tests.Models
{
    public class AppointmentRequestTests
    {
        private static List<ValidationResult> Validate(AppointmentRequest request)
        {
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(request, context, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void Valid_Request_Has_No_Errors()
        {
            var request = new AppointmentRequest
            {
                Name = "Philip",
                Email = "philip@example.com",
                Service = "Dental Cleaning",
                Gender = "Male",
                AppointmentDate = DateTime.Now.AddDays(3)
            };

            var results = Validate(request);

            Assert.Empty(results);
        }

        [Fact]
        public void Missing_Name_Fails_Validation()
        {
            var request = new AppointmentRequest
            {
                Name = "",
                Email = "philip@example.com",
                Service = "Dental Cleaning",
                Gender = "Male",
                AppointmentDate = DateTime.Now.AddDays(3)
            };

            var results = Validate(request);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AppointmentRequest.Name)));
        }

        [Fact]
        public void Invalid_Email_Fails_Validation()
        {
            var request = new AppointmentRequest
            {
                Name = "Philip",
                Email = "not-an-email",
                Service = "Dental Cleaning",
                Gender = "Male",
                AppointmentDate = DateTime.Now.AddDays(3)
            };

            var results = Validate(request);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AppointmentRequest.Email)));
        }

        [Fact]
        public void Past_Date_Fails_Validation()
        {
            var request = new AppointmentRequest
            {
                Name = "Philip",
                Email = "philip@example.com",
                Service = "Dental Cleaning",
                Gender = "Male",
                AppointmentDate = DateTime.Now.AddDays(-1)
            };

            var results = Validate(request);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("past"));
        }

        [Fact]
        public void Missing_Gender_Fails_Validation()
        {
            var request = new AppointmentRequest
            {
                Name = "Philip",
                Email = "philip@example.com",
                Service = "Dental Cleaning",
                Gender = "",
                AppointmentDate = DateTime.Now.AddDays(3)
            };

            var results = Validate(request);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AppointmentRequest.Gender)));
        }
    }
}
