using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using MyMvcApp.Models.Requests;
using MyMvcApp.Services;

namespace MyMvcApp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IBackgroundTaskQueue _taskQueue;

        public AppointmentController(
            ApplicationDbContext context,
            IEmailService emailService,
            IBackgroundTaskQueue taskQueue)
        {
            _context = context;
            _emailService = emailService;
            _taskQueue = taskQueue;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["AppointmentError"] = "Please correct the errors below and try again.";
                TempData["AppointmentErrors"] = string.Join(" | ",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                return RedirectToAction("Index", "Home");
            }

            bool alreadyBooked = await _context.Appointments
                .AnyAsync(a => a.Email == request.Email
                            && a.AppointmentDate.Date == request.AppointmentDate.Date);

            if (alreadyBooked)
            {
                TempData["AppointmentError"] = "You already have an appointment booked on this date.";
                return RedirectToAction("Index", "Home");
            }

            var appointment = new Appointment
            {
                Name = request.Name,
                Email = request.Email,
                Service = request.Service,
                Gender = request.Gender,
                AppointmentDate = request.AppointmentDate
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                await _emailService.SendAppointmentConfirmationAsync(appointment);
            });

            TempData["AppointmentSuccess"] = "Your appointment was booked successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
