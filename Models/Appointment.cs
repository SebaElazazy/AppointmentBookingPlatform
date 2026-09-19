using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        [ValidateNever]
        public ApplicationUser Customer { get; set; }

        public int StaffId { get; set; }
        [ValidateNever]
        public Staff Staff { get; set; }

        public int ServiceId { get; set; }
        [ValidateNever]
        public Service Service { get; set; }
        [ValidateNever]
        public DateTime AppointmentDate { get; set; }
        [ValidateNever]
        public TimeSpan StartTime { get; set; }
        [ValidateNever]
        public TimeSpan EndTime { get; set; }
        [ValidateNever]
        public AppointmentStatus Status { get; set; }
    }
}
