using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Interfaces;
using WebApplication.Infrastructure.Services;

namespace WebApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            // SQL OR SQL Lite can be configured depending upon the scenario.
            services.AddScoped<IUserService, UserService>();
            return services;
        }

        public static IServiceCollection ConfigureDevelopmentServices(this IServiceCollection services)
        {
            services.AddDbContext<InMemoryContext>();

            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
