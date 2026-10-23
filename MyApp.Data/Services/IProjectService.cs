using System;
using System.Collections.Generic;
using System.Text;
using MyApp.Shared.Dtos;

namespace MyApp.Data.Services
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetProjectsAsync();
        Task<ProjectDto?> GetProjectAsync(int projectId);
        Task<List<AssignmentDto>> GetAssignmentsByProjectAsync(int projectId);
        Task<List<EmployeeDto>> GetUnassignedEmployeesAsync(int projectId);
        Task<ServiceResult> AssignEmployeeAsync(AssignEmployeeDto dto);
        Task<int?> RemoveAssignmentAsync(int assignmentId); // retourne le ProjectId
    }
}
