using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.Shared.Dtos
{
    public class AssignEmployeeDto
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Veuillez choisir un employé.")]
        [Display(Name = "Employé")]
        public int? EmployeeId { get; set; }

        [Required(ErrorMessage = "Le rôle est obligatoire."), MaxLength(50)]
        [Display(Name = "Rôle")]
        public string Role { get; set; } = string.Empty;
    }
}
