using Microsoft.EntityFrameworkCore;
using MyApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Data
{
    public class SystemDbContext : DbContext
    {
        public SystemDbContext(DbContextOptions<SystemDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Assignment> Assignments => Set<Assignment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Configuration des relations ----------

            // Employee 1..* Assignment *..1 Project
            modelBuilder.Entity<Assignment>(a =>
            {
                a.HasOne(x => x.Employee)
                 .WithMany(e => e.Assignments)
                 .HasForeignKey(x => x.EmployeeId)
                 .OnDelete(DeleteBehavior.Cascade);

                a.HasOne(x => x.Project)
                 .WithMany(p => p.Assignments)
                 .HasForeignKey(x => x.ProjectId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Empêche d'affecter deux fois le même employé au même projet
                a.HasIndex(x => new { x.EmployeeId, x.ProjectId }).IsUnique();
            });

            modelBuilder.Entity<Skill>().HasIndex(s => s.Name).IsUnique();

            // ---------- Seeding (HasData) ----------

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FirstName = "Amine", LastName = "Benali", Email = "amine.benali@myapp.com", HireDate = new DateTime(2021, 3, 15) },
                new Employee { Id = 2, FirstName = "Sara", LastName = "Idrissi", Email = "sara.idrissi@myapp.com", HireDate = new DateTime(2020, 9, 1) },
                new Employee { Id = 3, FirstName = "Youssef", LastName = "Alaoui", Email = "youssef.alaoui@myapp.com", HireDate = new DateTime(2022, 1, 10) },
                new Employee { Id = 4, FirstName = "Nadia", LastName = "Tazi", Email = "nadia.tazi@myapp.com", HireDate = new DateTime(2023, 6, 5) }
            );

            modelBuilder.Entity<Skill>().HasData(
                new Skill { Id = 1, Name = "C#", Category = "Back-end" },
                new Skill { Id = 2, Name = "ASP.NET Core", Category = "Back-end" },
                new Skill { Id = 3, Name = "Blazor", Category = "Front-end" },
                new Skill { Id = 4, Name = "SQL", Category = "Base de données" },
                new Skill { Id = 5, Name = "JavaScript", Category = "Front-end" },
                new Skill { Id = 6, Name = "Docker", Category = "DevOps" }
            );

            modelBuilder.Entity<Project>().HasData(
                new Project { Id = 1, Name = "Portail RH", Description = "Gestion des ressources humaines", StartDate = new DateTime(2024, 1, 8) },
                new Project { Id = 2, Name = "E-Commerce", Description = "Boutique en ligne", StartDate = new DateTime(2024, 4, 1) },
                new Project { Id = 3, Name = "Application Mobile", Description = "Application de suivi client", StartDate = new DateTime(2025, 2, 17) }
            );

            modelBuilder.Entity<Assignment>().HasData(
                new Assignment { Id = 1, EmployeeId = 1, ProjectId = 1, Role = "Chef de projet", AssignedDate = new DateTime(2024, 1, 8) },
                new Assignment { Id = 2, EmployeeId = 2, ProjectId = 1, Role = "Développeuse", AssignedDate = new DateTime(2024, 1, 15) },
                new Assignment { Id = 3, EmployeeId = 3, ProjectId = 2, Role = "Développeur", AssignedDate = new DateTime(2024, 4, 1) },
                new Assignment { Id = 4, EmployeeId = 2, ProjectId = 2, Role = "Architecte", AssignedDate = new DateTime(2024, 5, 2) },
                new Assignment { Id = 5, EmployeeId = 4, ProjectId = 3, Role = "Testeuse", AssignedDate = new DateTime(2025, 2, 17) }
            );

            // Employee *..* Skill (table de jointure EmployeeSkills + seeding)
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Skills)
                .WithMany(s => s.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeSkill",
                    r => r.HasOne<Skill>().WithMany().HasForeignKey("SkillId"),
                    l => l.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"),
                    j =>
                    {
                        j.ToTable("EmployeeSkills");
                        j.HasKey("EmployeeId", "SkillId");
                        j.HasData(
                            new { EmployeeId = 1, SkillId = 1 },
                            new { EmployeeId = 1, SkillId = 2 },
                            new { EmployeeId = 1, SkillId = 4 },
                            new { EmployeeId = 2, SkillId = 1 },
                            new { EmployeeId = 2, SkillId = 3 },
                            new { EmployeeId = 3, SkillId = 5 },
                            new { EmployeeId = 3, SkillId = 6 },
                            new { EmployeeId = 4, SkillId = 4 }
                        );
                    });
        }
    }
}
