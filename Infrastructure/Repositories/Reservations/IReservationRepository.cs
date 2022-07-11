using Domain.Reservations;

namespace Infrastructure.Repositories.Reservations
{
    public interface IReservationRepository : IBaseRepository<Reservation>
    {
        Task<List<Reservation>> GetCanceledAsync();
        Task<List<Reservation>> GetCurrentReservationsAsync();
        Task<bool> AreReservationsInPeriodAsync(DateTime startDate, DateTime endDate);
        Task<List<Reservation>> GetReservationsByGuest(string guestName);
    }
}