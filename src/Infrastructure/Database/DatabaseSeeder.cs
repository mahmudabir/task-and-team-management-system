using Domain.Entities.Users;

using Infrastructure.Database.Users;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Settings;

namespace Infrastructure.Database;

public static class DatabaseSeeder
{
    public static async Task SeedApplicationBaseDataAsync(this WebApplication? app)
    {
        if (app == null)
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        try
        {
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            if (Convert.ToBoolean(configuration["SeedDataOnStartup"]))
            {
                await scope.SeedAdminUserData();
                await scope.SeedManagerUserData();
                await scope.SeedEmployeeUserData();
            }
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<IInfrastructureMarker>();
            logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }

    public static async Task SeedAdminUserData(this IServiceScope scope)
    {
        var adminRole = new ApplicationRole
        {
            Name = RoleService.AdminRoleName,
            NormalizedName = RoleService.AdminNormalizedRoleName
        };

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        if (!roleManager.Roles.Any(x => x.NormalizedName == adminRole.NormalizedName))
        {
            await roleManager.CreateAsync(adminRole);
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminSettings>>().Value;

        var adminUser = new ApplicationUser
        {
            UserName = adminOptions.Username,
            NormalizedUserName = adminOptions.Username?.ToUpper(),
            Email = adminOptions.Email,
            NormalizedEmail = adminOptions.Email?.ToUpper(),
            PhoneNumber = adminOptions.PhoneNumber?.ToUpper(),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            LockoutEnabled = false,
            TwoFactorEnabled = false
        };

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminOptions.Password!);

        if (!userManager.Users.Any(x => x.NormalizedUserName == adminUser.NormalizedUserName))
        {
            await userManager.CreateAsync(adminUser);
        }
        else
        {
            adminUser = await userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == adminUser.NormalizedUserName);
        }

        var usersInRole = await userManager.GetUsersInRoleAsync(adminRole.Name);
        if (!usersInRole.Select(x => x.NormalizedUserName).Contains(adminUser.NormalizedUserName))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole.Name);
        }

        adminRole = await roleManager.Roles.FirstOrDefaultAsync(x => x.NormalizedName == adminRole.NormalizedName);
        RoleService.AdminRoleId = adminRole!.Id;
    }

    public static async Task SeedManagerUserData(this IServiceScope scope)
    {
        var managerRole = new ApplicationRole
        {
            Name = RoleService.ManagerRoleName,
            NormalizedName = RoleService.ManagerNormalizedRoleName
        };

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        if (!roleManager.Roles.Any(x => x.NormalizedName == managerRole.NormalizedName))
        {
            await roleManager.CreateAsync(managerRole);
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var managerOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminSettings>>().Value;

        var managerUser = new ApplicationUser
        {
            UserName = managerOptions.Username,
            NormalizedUserName = managerOptions.Username?.ToUpper(),
            Email = managerOptions.Email,
            NormalizedEmail = managerOptions.Email?.ToUpper(),
            PhoneNumber = managerOptions.PhoneNumber?.ToUpper(),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            LockoutEnabled = false,
            TwoFactorEnabled = false
        };

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        managerUser.PasswordHash = passwordHasher.HashPassword(managerUser, managerOptions.Password!);

        if (!userManager.Users.Any(x => x.NormalizedUserName == managerUser.NormalizedUserName))
        {
            await userManager.CreateAsync(managerUser);
        }
        else
        {
            managerUser = await userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == managerUser.NormalizedUserName);
        }

        var usersInRole = await userManager.GetUsersInRoleAsync(managerRole.Name);
        if (!usersInRole.Select(x => x.NormalizedUserName).Contains(managerUser.NormalizedUserName))
        {
            await userManager.AddToRoleAsync(managerUser, managerRole.Name);
        }

        managerRole = await roleManager.Roles.FirstOrDefaultAsync(x => x.NormalizedName == managerRole.NormalizedName);
        RoleService.ManagerRoleId = managerRole!.Id;
    }


    public static async Task SeedEmployeeUserData(this IServiceScope scope)
    {
        var employeeRole = new ApplicationRole
        {
            Name = RoleService.EmployeeRoleName,
            NormalizedName = RoleService.EmployeeNormalizedRoleName
        };

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        if (!roleManager.Roles.Any(x => x.NormalizedName == employeeRole.NormalizedName))
        {
            await roleManager.CreateAsync(employeeRole);
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var employeeOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminSettings>>().Value;

        var employeeUser = new ApplicationUser
        {
            UserName = employeeOptions.Username,
            NormalizedUserName = employeeOptions.Username?.ToUpper(),
            Email = employeeOptions.Email,
            NormalizedEmail = employeeOptions.Email?.ToUpper(),
            PhoneNumber = employeeOptions.PhoneNumber?.ToUpper(),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            LockoutEnabled = false,
            TwoFactorEnabled = false
        };

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        employeeUser.PasswordHash = passwordHasher.HashPassword(employeeUser, employeeOptions.Password!);

        if (!userManager.Users.Any(x => x.NormalizedUserName == employeeUser.NormalizedUserName))
        {
            await userManager.CreateAsync(employeeUser);
        }
        else
        {
            employeeUser = await userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == employeeUser.NormalizedUserName);
        }

        var usersInRole = await userManager.GetUsersInRoleAsync(employeeRole.Name);
        if (!usersInRole.Select(x => x.NormalizedUserName).Contains(employeeUser.NormalizedUserName))
        {
            await userManager.AddToRoleAsync(employeeUser, employeeRole.Name);
        }

        employeeRole = await roleManager.Roles.FirstOrDefaultAsync(x => x.NormalizedName == employeeRole.NormalizedName);
        RoleService.EmployeeRoleId = employeeRole!.Id;
    }
}