using Microsoft.AspNetCore.Identity;
using OnlineCafe.Storage;

namespace OnlineCafe.Services.Configuratons
{
    public static class ConfigureIdentity
    {
        public static async Task ConfigureIdentityAsync(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();
            var config = app.Configuration.GetSection("DefaultUsersConfig");
            var adminRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Administrator);
            if (adminRole == null)
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = ApplicationRoleNames.Administrator,
                    Type = RoleType.Administrator
                });
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {ApplicationRoleNames.Administrator} role.");
                }

                adminRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Administrator);
            }

            var adminUser = await userManager.FindByNameAsync(config["AdminUserName"]);
            if (adminUser == null)
            {
                var userResult = await userManager.CreateAsync(new User
                {
                    UserName = config["AdminUserName"],
                    Email = config["AdminUserName"],
                    Name = "Nibi Java",
                    BirthDate = new DateTime(1992, 1, 1),
                    Phone = "657567567676"
                }, config["AdminPassword"]);
                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {config["AdminUserName"]} user");
                }

                adminUser = await userManager.FindByNameAsync(config["AdminUserName"]);
            }

            if (!await userManager.IsInRoleAsync(adminUser, adminRole.Name))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole.Name);
            }

            var userRole = await roleManager.FindByNameAsync(ApplicationRoleNames.User);
            if (userRole == null)
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = ApplicationRoleNames.User,
                    Type = RoleType.User
                });
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {ApplicationRoleNames.User} role.");
                }
            }
        }
    }
}