namespace AppointmentsManagementSystem.Models
{
    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }

    public class Appointment
    {
        public int Id { get; set; }

        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
