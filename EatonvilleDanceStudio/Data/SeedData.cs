using Microsoft.AspNetCore.Identity;
using EatonvilleDanceStudio.Models;

namespace EatonvilleDanceStudio.Data;

/// <summary>
/// Provides functionality for seeding initial data into the application's database during startup.
/// This includes setting up foundational roles, creating a master account, and provisioning
/// starter dance classes for a fresh database.
/// </summary>

public class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        // Resolve the Identity managers AND the Database Context from the dependency injection container
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Define and create the foundational roles for the studio
        string[] roleNames = { "Director", "Instructor", "Parent", "Dancer" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Define your master account email TESTING
        // UPDATE THIS TO THE EMAIL YOU WANT TO USE: IT WILL GET DELETED LATER
        var directorEmail = "zues253@gmail.com"; 

        var directorUser = await userManager.FindByEmailAsync(directorEmail);
        
        if (directorUser == null)
        {
            // If you haven't registered this account yet, create it and promote it
            directorUser = new ApplicationUser
            {
                UserName = directorEmail,
                Email = directorEmail,
                EmailConfirmed = true
            };
            
            // Creates the user with a valid Identity password
            var createPowerUser = await userManager.CreateAsync(directorUser, "EatonvilleDanceStudio1!");
            
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(directorUser, "Director");
            }
            else 
            {
                // If the password or creation fails, print the exact reason to the console
                foreach (var error in createPowerUser.Errors)
                {
                    Console.WriteLine($"Failed to create user: {error.Description}");
                }
            }
        }
        else
        {
            // Force Confirm the email 
            if (!directorUser.EmailConfirmed)
            {
                directorUser.EmailConfirmed = true;
                await userManager.UpdateAsync(directorUser);
            }
            
            // Promote it to Director
            if (!await userManager.IsInRoleAsync(directorUser, "Director"))
            {
                await userManager.AddToRoleAsync(directorUser, "Director");
            }
        } 

        // Provision starter Dance Classes for fresh database deployments
        if (!context.DanceClasses.Any())
        {
            var starterClasses = new List<DanceClass>
            {
                new DanceClass
                {
                    ClassName = "Tiny Tots Ballet",
                    Description = "A fun, introductory ballet class focusing on basic movement and rhythm.",
                    MinimumAge = 3,
                    MaximumAge = 5,
                    RequiredSkillLevel = "Beginner",
                    Capacity = 12,
                    DayOfWeek = "Monday",
                    StartTime = new TimeSpan(15, 30, 0), // 3:30 PM
                    EndTime = new TimeSpan(16, 15, 0)    // 4:15 PM
                },
                new DanceClass
                {
                    ClassName = "Intermediate Hip Hop",
                    Description = "High-energy hip hop routine focusing on choreography and stage presence.",
                    MinimumAge = 10,
                    MaximumAge = 15,
                    RequiredSkillLevel = "Intermediate",
                    Capacity = 20,
                    DayOfWeek = "Wednesday",
                    StartTime = new TimeSpan(17, 0, 0),  // 5:00 PM
                    EndTime = new TimeSpan(18, 0, 0)     // 6:00 PM
                }
            };

            context.DanceClasses.AddRange(starterClasses);
            await context.SaveChangesAsync();
        }
    } 
}