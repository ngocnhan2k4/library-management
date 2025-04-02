using LibraryManagement.Core.Constants;
using LibraryManagement.Core.Identity;
using LibraryManagement.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Seed Roles
                await SeedRoles(roleManager);

                // Seed Admin User
                await SeedAdminUser(userManager);

                // Seed Categories and Books
                await SeedBooks(context);
            }
        }

        private static async Task SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            // Tạo các Roles
            await CreateRoleIfNotExists(roleManager, new ApplicationRole
            {
                Name = Roles.Admin,
                Description = "Full access to all features",
                CreatedDate = DateTime.Now
            });

            await CreateRoleIfNotExists(roleManager, new ApplicationRole
            {
                Name = Roles.Librarian,
                Description = "Manage books and loans",
                CreatedDate = DateTime.Now
            });

            await CreateRoleIfNotExists(roleManager, new ApplicationRole
            {
                Name = Roles.Member,
                Description = "Borrow books",
                CreatedDate = DateTime.Now
            });

            await CreateRoleIfNotExists(roleManager, new ApplicationRole
            {
                Name = Roles.Guest,
                Description = "Browse books only",
                CreatedDate = DateTime.Now
            });
        }
        private static async Task CreateRoleIfNotExists(
            RoleManager<ApplicationRole> roleManager, ApplicationRole role)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(role);
            }
        }

        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@library.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Address = "Library HQ"
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);

                // Add claims to admin
                await userManager.AddClaimAsync(adminUser,
                    new Claim(Claims.ManageInventory, "true"));
                await userManager.AddClaimAsync(adminUser,
                    new Claim(Claims.ManageLoans, "true"));
                await userManager.AddClaimAsync(adminUser,
                    new Claim(Claims.ViewReports, "true"));
                await userManager.AddClaimAsync(adminUser,
                    new Claim(Claims.ApproveReturns, "true"));
            }
        }

        private static async Task SeedBooks(ApplicationDbContext context)
        {
            // Seed categories if none exist
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Fiction" },
                    new Category { Name = "Science" },
                    new Category { Name = "History" },
                    new Category { Name = "Technology" }
                );   
                await context.SaveChangesAsync();
            }

            // Seed books if none exist
            if (!context.Books.Any())
            {
                context.Books.AddRange(
                    new Book
                    {
                        Title = "Clean Code",
                        Author = "Robert C. Martin",
                        ISBN = "9780132350884",
                        PublicationYear = 2008,
                        IsAvailable = true,
                        CategoryId = 1
                    },
                    new Book
                    {
                        Title = "To Kill a Mockingbird",
                        Author = "Harper Lee",
                        ISBN = "9780061120084",
                        PublicationYear = 1960,
                        IsAvailable = true,
                        CategoryId = 2
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
