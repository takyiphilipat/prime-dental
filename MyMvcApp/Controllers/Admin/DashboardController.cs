using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models.Admin;

namespace MyMvcApp.Controllers.Admin
{
    [Authorize]
    [Route("Admin/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Now.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var totalAppointments = await _context.Appointments.CountAsync();
            var appointmentsToday = await _context.Appointments
                .CountAsync(a => a.AppointmentDate.Date == today);
            var appointmentsThisMonth = await _context.Appointments
                .CountAsync(a => a.AppointmentDate >= startOfMonth && a.AppointmentDate < startOfNextMonth);
            var totalPosts = await _context.Posts.CountAsync();

            // Last 14 days, appointment counts per day (for the trend chart)
            var fourteenDaysAgo = today.AddDays(-13);
            var dailyCounts = await _context.Appointments
                .Where(a => a.AppointmentDate.Date >= fourteenDaysAgo && a.AppointmentDate.Date <= today)
                .GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            // Fill in missing days with 0, so the chart doesn't have gaps
            var chartLabels = new List<string>();
            var chartValues = new List<int>();
            for (var date = fourteenDaysAgo; date <= today; date = date.AddDays(1))
            {
                chartLabels.Add(date.ToString("MMM d"));
                chartValues.Add(dailyCounts.FirstOrDefault(d => d.Date == date)?.Count ?? 0);
            }

            var viewModel = new DashboardViewModel
            {
                TotalAppointments = totalAppointments,
                AppointmentsToday = appointmentsToday,
                AppointmentsThisMonth = appointmentsThisMonth,
                TotalPosts = totalPosts,
                ChartLabels = chartLabels,
                ChartValues = chartValues
            };

            return View("~/Views/Admin/Dashboard/Index.cshtml", viewModel);
        }
    }
}
