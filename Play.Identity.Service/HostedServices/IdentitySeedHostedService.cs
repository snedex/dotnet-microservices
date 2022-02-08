using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Play.Identity.Service.Areas.Identity.Entites;
using Play.Identity.Service.Settings;

namespace Play.Identity.Service.HostedServices
{
    public class IdentitySeedHostedService : IHostedService
    {
        private readonly IServiceScopeFactory factory;
        private readonly IdentitySettings settings;

        public IdentitySeedHostedService(IServiceScopeFactory factory, IOptions<IdentitySettings> settings)
        {
            this.settings = settings.Value;
            this.factory = factory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = factory.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await CreateRoleIfNotExistsAsync(Roles.Admin, roleManager);
            await CreateRoleIfNotExistsAsync(Roles.Player, roleManager);

            var adminUser = await userManager.FindByEmailAsync(settings.AdminUserName);

            if(adminUser == null)
            {
                adminUser = new ApplicationUser {
                    UserName = settings.AdminUserName,
                    Email = settings.AdminUserName
                };

                await userManager.CreateAsync(adminUser, settings.AdminUserPassword);

                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private static async Task CreateRoleIfNotExistsAsync(string roleName, RoleManager<ApplicationRole> manager)
        {
            var exists = await manager.RoleExistsAsync(roleName);
            
            if(!exists)
            {
                await manager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }
    }
}