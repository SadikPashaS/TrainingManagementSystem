using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Enums;
using TrainingManagement.Infrastructure.Data;
using BCrypt.Net;

namespace TrainingManagement.API.Data
{
    public static class TestDataSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if any users exist
            if (!context.Users.Any())
            {
                // Add Admin User - NO ID specified
                context.Users.Add(new User
                {
                    UserId = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@training.com",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

                // Add Normal User - NO ID specified
                context.Users.Add(new User
                {
                    UserId = "john.doe",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@training.com",
                    Role = UserRole.NormalUser,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            // Check if any trainings exist
            if (!context.Trainings.Any())
            {
                // Add sample trainings - NO ID specified
                context.Trainings.AddRange(
                    new Training
                    {
                        TrainingName = "Cybersecurity Basics",
                        TrainingUrl = "https://training.com/cybersecurity",
                        Mode = TrainingMode.Virtual,
                        Platform = "Zoom",
                        ExpectedStartDate = DateTime.UtcNow.AddDays(7),
                        ExpectedEndDate = DateTime.UtcNow.AddDays(14),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Training
                    {
                        TrainingName = "Data Privacy Guidelines",
                        TrainingUrl = "https://training.com/dataprivacy",
                        Mode = TrainingMode.InstructorLed,
                        Platform = "Teams",
                        ExpectedStartDate = DateTime.UtcNow.AddDays(5),
                        ExpectedEndDate = DateTime.UtcNow.AddDays(12),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Training
                    {
                        TrainingName = "Project Management",
                        TrainingUrl = "https://training.com/projectmanagement",
                        Mode = TrainingMode.Virtual,
                        Platform = "Webex",
                        ExpectedStartDate = DateTime.UtcNow.AddDays(10),
                        ExpectedEndDate = DateTime.UtcNow.AddDays(20),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Training
                    {
                        TrainingName = "Workplace Safety",
                        TrainingUrl = "https://training.com/safety",
                        Mode = TrainingMode.InstructorLed,
                        Platform = "InPerson",
                        ExpectedStartDate = DateTime.UtcNow.AddDays(3),
                        ExpectedEndDate = DateTime.UtcNow.AddDays(4),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                );

                context.SaveChanges();
            }

            // Check if any user-trainings exist
            if (!context.UserTrainings.Any())
            {
                var normalUser = context.Users.FirstOrDefault(u => u.UserId == "john.doe");
                var trainings = context.Trainings.ToList();

                if (normalUser != null && trainings.Count >= 3)
                {
                    // Assign trainings - NO ID specified
                    context.UserTrainings.AddRange(
                        new UserTraining
                        {
                            UserId = normalUser.Id,
                            TrainingId = trainings[0].Id,
                            Status = TrainingStatus.InProgress,
                            AssignedAt = DateTime.UtcNow.AddDays(-5)
                        },
                        new UserTraining
                        {
                            UserId = normalUser.Id,
                            TrainingId = trainings[1].Id,
                            Status = TrainingStatus.Delayed,
                            ExpectedCompletionDate = DateTime.UtcNow.AddDays(10),
                            Comments = "Need more time due to workload",
                            AssignedAt = DateTime.UtcNow.AddDays(-10)
                        },
                        new UserTraining
                        {
                            UserId = normalUser.Id,
                            TrainingId = trainings[2].Id,
                            Status = TrainingStatus.Completed,
                            AssignedAt = DateTime.UtcNow.AddDays(-20)
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}