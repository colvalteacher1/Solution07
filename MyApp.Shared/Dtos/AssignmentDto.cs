using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Shared.Dtos
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public int SkillCount { get; set; }
    }
}
