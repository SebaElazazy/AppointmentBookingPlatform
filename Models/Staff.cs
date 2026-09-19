using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AppointmentsManagementSystem.Models
{
    public class Staff
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ValidateNever]
        public ApplicationUser User { get; set; }

        public string Specialization { get; set; }
        public string ?ImageUrl { get; set; }

        [ValidateNever]
        public ICollection<StaffService> StaffServices { get; set; } = new List<StaffService>();
        [ValidateNever]

        public ICollection<WorkingHour> WorkingHours { get; set; }= new List<WorkingHour>();
        [ValidateNever]
        public ICollection<AvailabilityException> AvailabilityExceptions { get; set; }= new List<AvailabilityException>();
        [ValidateNever]
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
