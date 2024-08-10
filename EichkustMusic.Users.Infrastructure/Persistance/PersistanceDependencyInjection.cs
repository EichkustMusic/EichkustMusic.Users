using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EichkustMusic.Users.Infrastructure.Persistance
{
    public static class PersistanceDependencyInjection
    {
        public static IServiceCollection AddPersistace(
            this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("UsersDb")));

            return services;
        }
    }
}
