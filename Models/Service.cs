using System.ComponentModel.DataAnnotations.Schema;
namespace AppointmentsManagementSystem.Models
{
   

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int DurationMinutes { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }

        public ICollection<StaffService> StaffServices { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
