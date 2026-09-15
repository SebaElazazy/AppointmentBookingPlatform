namespace AppointmentsManagementSystem.Models
{
    public class StaffService
    {
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
