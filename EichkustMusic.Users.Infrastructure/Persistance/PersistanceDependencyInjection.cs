using EichkustMusic.Users.Application.UserRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EichkustMusic.Users.Infrastructure.Persistance
{
    public static class PersistanceDependencyInjection
    {
        public static IServiceCollection AddPersistace(
            this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("UsersDb")));

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
