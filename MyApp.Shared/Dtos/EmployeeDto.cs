using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Shared.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int SkillCount { get; set; }
    }
}
