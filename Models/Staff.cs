namespace AppointmentsManagementSystem.Models
{
    public class Staff
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Specialization { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<StaffService> StaffServices { get; set; }
        public ICollection<WorkingHour> WorkingHours { get; set; }
        public ICollection<AvailabilityException> AvailabilityExceptions { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
