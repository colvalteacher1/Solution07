using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Shared.Dtos
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
    }
}
