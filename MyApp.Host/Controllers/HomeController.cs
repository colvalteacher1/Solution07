using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.Data.Services;
using MyApp.Shared.Dtos;
using System.Diagnostics;

namespace MyApp.Host.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProjectService _projects;
        private readonly IEmployeeService _employees;

        public HomeController(IProjectService projects, IEmployeeService employees)
        {
            _projects = projects;
            _employees = employees;
        }

        // Liste des projets + nombre d'employés
        public async Task<IActionResult> Index()
            => View(await _projects.GetProjectsAsync());

        // Employés d'un projet (rôle, date, nb compétences)
        public async Task<IActionResult> EmployeesByProject(int id)
        {
            var project = await _projects.GetProjectAsync(id);
            if (project is null) return NotFound();

            ViewBag.Project = project;
            return View(await _projects.GetAssignmentsByProjectAsync(id));
        }

        // Compétences d'un employé
        public async Task<IActionResult> SkillsByEmployee(int id)
        {
            var employee = await _employees.GetEmployeeAsync(id);
            if (employee is null) return NotFound();

            ViewBag.Employee = employee;
            return View(await _employees.GetSkillsByEmployeeAsync(id));
        }

        // ---------- AddSkill ----------
        [HttpGet]
        public async Task<IActionResult> AddSkill(int id)
        {
            var employee = await _employees.GetEmployeeAsync(id);
            if (employee is null) return NotFound();

            await LoadAddSkillData(employee);
            return View(new AddSkillDto { EmployeeId = id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSkill(AddSkillDto dto)
        {
            var employee = await _employees.GetEmployeeAsync(dto.EmployeeId);
            if (employee is null) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _employees.AddSkillAsync(dto);
                if (result.Success)
                    return RedirectToAction(nameof(SkillsByEmployee), new { id = dto.EmployeeId });

                ModelState.AddModelError(string.Empty, result.Error!);
            }

            await LoadAddSkillData(employee);
            return View(dto);
        }

        private async Task LoadAddSkillData(EmployeeDto employee)
        {
            ViewBag.Employee = employee;
            var missing = await _employees.GetMissingSkillsAsync(employee.Id);
            ViewBag.Skills = new SelectList(missing, nameof(SkillDto.Id), nameof(SkillDto.Name));
        }

        // ---------- AssignEmployee ----------
        [HttpGet]
        public async Task<IActionResult> AssignEmployee(int id)
        {
            var project = await _projects.GetProjectAsync(id);
            if (project is null) return NotFound();

            await LoadAssignData(project);
            return View(new AssignEmployeeDto { ProjectId = id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEmployee(AssignEmployeeDto dto)
        {
            var project = await _projects.GetProjectAsync(dto.ProjectId);
            if (project is null) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _projects.AssignEmployeeAsync(dto);
                if (result.Success)
                    return RedirectToAction(nameof(EmployeesByProject), new { id = dto.ProjectId });

                ModelState.AddModelError(string.Empty, result.Error!);
            }

            await LoadAssignData(project);
            return View(dto);
        }

        private async Task LoadAssignData(ProjectDto project)
        {
            ViewBag.Project = project;
            var available = await _projects.GetUnassignedEmployeesAsync(project.Id);
            ViewBag.Employees = new SelectList(available, nameof(EmployeeDto.Id), nameof(EmployeeDto.FullName));
        }

        // ---------- RemoveAssignment ----------
        // Formulaire POST rendu par le composant Blazor statique (pas de jeton antiforgery)
        [HttpPost]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            var projectId = await _projects.RemoveAssignmentAsync(id);
            if (projectId is null) return NotFound();

            return RedirectToAction(nameof(EmployeesByProject), new { id = projectId });
        }
    }
}
