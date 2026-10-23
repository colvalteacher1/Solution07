using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.Shared.Dtos
{
    public class AddSkillDto
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Veuillez choisir une compétence.")]
        [Display(Name = "Compétence")]
        public int? SkillId { get; set; }
    }
}
