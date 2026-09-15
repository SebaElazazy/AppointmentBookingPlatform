namespace AppointmentsManagementSystem.Models
{
    public class AvailabilityException
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public DateTime Date { get; set; }
        public bool IsDayOff { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
