namespace AppointmentsManagementSystem.Models
{
    public class WorkingHour
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
