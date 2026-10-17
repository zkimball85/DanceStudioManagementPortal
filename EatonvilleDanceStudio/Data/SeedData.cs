using Microsoft.AspNetCore.Identity;
using EatonvilleDanceStudio.Models;

namespace EatonvilleDanceStudio.Data;

public class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        // Resolve the Identity managers from the dependency injection container
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Define and create the foundational roles for the studio
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
    } 
}