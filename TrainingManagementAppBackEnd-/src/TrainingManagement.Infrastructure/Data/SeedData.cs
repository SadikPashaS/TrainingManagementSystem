using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Enums;
using BCrypt.Net;  // Make sure this is here

namespace TrainingManagement.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Wait for database to be ready
                await context.Database.MigrateAsync();

                // Seed Users
                if (!context.Users.Any())
                {
                    logger.LogInformation("Seeding users...");

                    var adminUser = new User
                    {
                        UserId = "admin",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),  // Fully qualified
                        FirstName = "System",
                        LastName = "Admin",
                        Email = "admin@training.com",
                        Role = UserRole.Admin,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var normalUser = new User
                    {
                        UserId = "john.doe",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),  // Fully qualified
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john@training.com",
                        Role = UserRole.NormalUser,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Users.AddRangeAsync(adminUser, normalUser);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Users seeded successfully");
                }

                // Rest of your code remains the same...
                // Seed Trainings
                if (!context.Trainings.Any())
                {
                    logger.LogInformation("Seeding trainings...");

                    var trainings = new[]
                    {
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
                    };

                    await context.Trainings.AddRangeAsync(trainings);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Trainings seeded successfully");
                }

                // Seed UserTrainings
                if (!context.UserTrainings.Any())
                {
                    logger.LogInformation("Seeding user trainings...");

                    var normalUser = await context.Users.FirstOrDefaultAsync(u => u.UserId == "john.doe");
                    var trainings = await context.Trainings.ToListAsync();

                    if (normalUser != null && trainings.Count >= 4)
                    {
                        var userTrainings = new List<UserTraining>
                        {
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
                            },
                            new UserTraining
                            {
                                UserId = normalUser.Id,
                                TrainingId = trainings[3].Id,
                                Status = TrainingStatus.Pending,
                                AssignedAt = DateTime.UtcNow.AddDays(-1)
                            }
                        };

                        await context.UserTrainings.AddRangeAsync(userTrainings);
                        await context.SaveChangesAsync();
                        logger.LogInformation("User trainings seeded successfully");
                    }
                }

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}