using MyApp.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Data.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetEmployeeAsync(int employeeId);
        Task<List<SkillDto>> GetSkillsByEmployeeAsync(int employeeId);
        Task<List<SkillDto>> GetMissingSkillsAsync(int employeeId);
        Task<ServiceResult> AddSkillAsync(AddSkillDto dto);
    }
}
