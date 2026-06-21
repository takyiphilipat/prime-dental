using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models.Requests
{
    public class AppointmentRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a service")]
        public string Service { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select your gender")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AppointmentDate.Date < DateTime.Now.Date)
            {
                yield return new ValidationResult(
                    "Appointment date cannot be in the past.",
                    new[] { nameof(AppointmentDate) });
            }
        }
    }
}
