using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }

        [ValidateNever]
        public ICollection<StaffService> StaffServices { get; set; } = new List<StaffService>();

        [ValidateNever]
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}