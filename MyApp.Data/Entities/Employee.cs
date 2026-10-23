using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public DateTime HireDate { get; set; }

        // Navigation
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
