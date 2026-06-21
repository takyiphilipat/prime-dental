using MyMvcApp.Models;

namespace MyMvcApp.Services
{
    public interface IEmailService
    {
        Task SendAppointmentConfirmationAsync(Appointment appointment);
    }
}
