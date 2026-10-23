using Microsoft.EntityFrameworkCore;
using MyApp.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Data.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly SystemDbContext _db;
        public EmployeeService(SystemDbContext db) => _db = db;

        public Task<EmployeeDto?> GetEmployeeAsync(int employeeId) =>
            _db.Employees.AsNoTracking()
                .Where(e => e.Id == employeeId)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = e.FirstName + " " + e.LastName,
                    Email = e.Email,
                    SkillCount = e.Skills.Count()
                })
                .FirstOrDefaultAsync();

        public Task<List<SkillDto>> GetSkillsByEmployeeAsync(int employeeId) =>
            _db.Skills.AsNoTracking()
                .Where(s => s.Employees.Any(e => e.Id == employeeId))
                .OrderBy(s => s.Name)
                .Select(s => new SkillDto { Id = s.Id, Name = s.Name, Category = s.Category })
                .ToListAsync();

        public Task<List<SkillDto>> GetMissingSkillsAsync(int employeeId) =>
            _db.Skills.AsNoTracking()
                .Where(s => !s.Employees.Any(e => e.Id == employeeId))
                .OrderBy(s => s.Name)
                .Select(s => new SkillDto { Id = s.Id, Name = s.Name, Category = s.Category })
                .ToListAsync();

        public async Task<ServiceResult> AddSkillAsync(AddSkillDto dto)
        {
            if (dto.SkillId is null)
                return ServiceResult.Fail("Veuillez choisir une compétence.");

            var employee = await _db.Employees
                .Include(e => e.Skills)
                .FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
            if (employee is null)
                return ServiceResult.Fail("Employé introuvable.");

            if (employee.Skills.Any(s => s.Id == dto.SkillId))
                return ServiceResult.Fail("L'employé possède déjà cette compétence.");

            var skill = await _db.Skills.FindAsync(dto.SkillId.Value);
            if (skill is null)
                return ServiceResult.Fail("Compétence introuvable.");

            employee.Skills.Add(skill);
            await _db.SaveChangesAsync();
            return ServiceResult.Ok();
        }
    }
}
