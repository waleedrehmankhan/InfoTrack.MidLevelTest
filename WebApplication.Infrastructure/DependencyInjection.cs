using Microsoft.Extensions.DependencyInjection;
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
            services.AddDbContext<DataContext>();
            ConfigureCommonServices(services);
            return services;
        }

        public static IServiceCollection ConfigureDevelopmentServices(this IServiceCollection services)
        {
            services.AddDbContext<InMemoryContext>();
            ConfigureCommonServices(services);
            return services;
        }

        private static void ConfigureCommonServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
        }
    }
}
