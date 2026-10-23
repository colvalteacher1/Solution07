using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.Data.Entities
{
    public class Assignment
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }
    }
}
