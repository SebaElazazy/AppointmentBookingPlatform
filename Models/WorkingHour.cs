using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AppointmentsManagementSystem.Models
{
    public class WorkingHour
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        [ValidateNever]
        public Staff Staff { get; set; }
        [ValidateNever]
        public DayOfWeek DayOfWeek { get; set; }
        [ValidateNever]
        public TimeSpan StartTime { get; set; }
        [ValidateNever]
        public TimeSpan EndTime { get; set; }
    }
}
