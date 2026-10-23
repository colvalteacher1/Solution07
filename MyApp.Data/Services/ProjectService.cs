using Microsoft.EntityFrameworkCore;
using MyApp.Data.Entities;
using MyApp.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Data.Services
{
    public class ProjectService : IProjectService
    {
        private readonly SystemDbContext _db;
        public ProjectService(SystemDbContext db) => _db = db;

        public Task<List<ProjectDto>> GetProjectsAsync() =>
            _db.Projects.AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    EmployeeCount = p.Assignments.Count()
                })
                .ToListAsync();

        public Task<ProjectDto?> GetProjectAsync(int projectId) =>
            _db.Projects.AsNoTracking()
                .Where(p => p.Id == projectId)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    EmployeeCount = p.Assignments.Count()
                })
                .FirstOrDefaultAsync();

        public Task<List<AssignmentDto>> GetAssignmentsByProjectAsync(int projectId) =>
            _db.Assignments.AsNoTracking()
                .Where(a => a.ProjectId == projectId)
                .OrderBy(a => a.Employee.LastName)
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                    ProjectId = a.ProjectId,
                    Role = a.Role,
                    AssignedDate = a.AssignedDate,
                    SkillCount = a.Employee.Skills.Count()
                })
                .ToListAsync();

        public Task<List<EmployeeDto>> GetUnassignedEmployeesAsync(int projectId) =>
            _db.Employees.AsNoTracking()
                .Where(e => !e.Assignments.Any(a => a.ProjectId == projectId))
                .OrderBy(e => e.LastName)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = e.FirstName + " " + e.LastName,
                    Email = e.Email,
                    SkillCount = e.Skills.Count()
                })
                .ToListAsync();

        public async Task<ServiceResult> AssignEmployeeAsync(AssignEmployeeDto dto)
        {
            if (dto.EmployeeId is null)
                return ServiceResult.Fail("Veuillez choisir un employé.");

            if (!await _db.Projects.AnyAsync(p => p.Id == dto.ProjectId))
                return ServiceResult.Fail("Projet introuvable.");

            if (!await _db.Employees.AnyAsync(e => e.Id == dto.EmployeeId))
                return ServiceResult.Fail("Employé introuvable.");

            // Règle métier : pas de double affectation
            bool already = await _db.Assignments
                .AnyAsync(a => a.ProjectId == dto.ProjectId && a.EmployeeId == dto.EmployeeId);
            if (already)
                return ServiceResult.Fail("Cet employé est déjà affecté à ce projet.");

            _db.Assignments.Add(new Assignment
            {
                ProjectId = dto.ProjectId,
                EmployeeId = dto.EmployeeId.Value,
                Role = dto.Role.Trim(),
                AssignedDate = DateTime.Today
            });
            await _db.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<int?> RemoveAssignmentAsync(int assignmentId)
        {
            var assignment = await _db.Assignments.FindAsync(assignmentId);
            if (assignment is null) return null;

            int projectId = assignment.ProjectId;
            _db.Assignments.Remove(assignment);
            await _db.SaveChangesAsync();
            return projectId;
        }
    }
}
