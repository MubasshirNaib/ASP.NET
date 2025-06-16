using Backend.Application;
using Backend.Infrastructure;

namespace Backend.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {

            services.AddApplicationDI()
                .AddInfrastructureDI();
            return services;
        }
    }
}
