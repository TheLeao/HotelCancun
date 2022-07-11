using Infrastructure.Repositories;
using Infrastructure.Repositories.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure
{
    public static class Configuration
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options => options
                .UseInMemoryDatabase("Hotel")
                .ConfigureWarnings(c => c.Ignore(InMemoryEventId.TransactionIgnoredWarning)),
                ServiceLifetime.Singleton);

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IReservationRepository, ReservationRepository>();
            return services;
        }
    }
}
