namespace MyMvcApp.Models.Admin
{
    public class DashboardViewModel
    {
        public int TotalAppointments { get; set; }
        public int AppointmentsToday { get; set; }
        public int AppointmentsThisMonth { get; set; }
        public int TotalPosts { get; set; }
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartValues { get; set; } = new();
    }
}
