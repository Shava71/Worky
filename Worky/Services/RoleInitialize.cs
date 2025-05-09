using Microsoft.AspNetCore.Identity;
using Worky.Migrations;
using Worky.Models;

namespace Worky.Services;

public static class RoleInitialize
{
    public static async Task Initialize(RoleManager<Roles> roleManager, ILogger logger)
    {
        List<string> roles = new List<string>()
        {
            MyRoles.SuperAdmin.ToString(),
            MyRoles.Manager.ToString(),
            MyRoles.Company.ToString(),
            MyRoles.Worker.ToString(),
        };

        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                logger.LogInformation($"Role {role} does not exist. Creating...");

                Roles newRole = new Roles()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = role,
                    NormalizedName = role.ToUpper(),
                    
                };
                logger.LogInformation($"RoleID: {newRole.Id}");

                var result = await roleManager.CreateAsync(newRole);

                if (result.Succeeded)
                {
                    logger.LogInformation($"Role {role} created successfully.");
                }
                else
                {
                    logger.LogError($"Failed to create role {role}: {string.Join(", ", result.Errors)}");
                }
            }
            else
            {
                logger.LogInformation($"Role {role} already exists.");
            }
        }
    }
}