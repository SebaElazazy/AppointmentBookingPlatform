using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AppointmentsManagementSystem.Models
{
    public class AvailabilityException
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        [ValidateNever]
        public Staff Staff { get; set; }
        [ValidateNever]
        public DateTime Date { get; set; }
        public bool IsDayOff { get; set; }
        [ValidateNever]
        public TimeSpan? StartTime { get; set; }
        [ValidateNever]
        public TimeSpan? EndTime { get; set; }
    }
}
