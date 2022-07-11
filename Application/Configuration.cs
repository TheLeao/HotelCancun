using Application.Reservations.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class Configuration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IReservationService, ReservationService>();
            return services;
        }
    }
}
