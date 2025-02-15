using Management.Application;
using Management.Infrastructure;

namespace Management.Api
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
