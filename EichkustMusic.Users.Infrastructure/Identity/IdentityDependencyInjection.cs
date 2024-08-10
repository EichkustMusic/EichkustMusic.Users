using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using EichkustMusic.Users.Domain.Entities;
using EichkustMusic.Users.Infrastructure.Persistance;

namespace EichkustMusic.Users.Infrastructure.Identity
{
    public static class IdentityDependencyInjection
    {
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<int>>()
                .AddEntityFrameworkStores<UsersDbContext>();

            var secretKey = configuration["IdentityServer:SecretKey"];

            if (secretKey == null)
            {
                throw new Exception("Secret key is null");
            }

            var identityConfiguration = new IdentityConfiguration(secretKey);

            services.AddIdentityServer()
                .AddInMemoryClients(identityConfiguration.Clients)
                .AddInMemoryIdentityResources(identityConfiguration.IdentityResources)
                .AddInMemoryApiResources(identityConfiguration.Apis)
                .AddInMemoryApiScopes(identityConfiguration.Scopes)
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();

            return services;
        }
    }
}
