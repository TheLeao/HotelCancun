using Domain.Reservations;

namespace Application.Reservations.Services
{
    public interface IReservationService
    {
        Task<Reservation?> GetReservationAsync(long reservationId);
        Task<List<Reservation>> GetReservationsAsync();
        Task<List<Reservation>> GetCancelledReservationsAsync();
        Task<bool> CheckRoomAvailabilityAsync(DateTime startDate, DateTime endDate);
        Task<ReservationOperationResult> CancelReservationAsync(long reservationId);
        Task<ReservationOperationResult> ModifyReservationAsync(long reservationId, Reservation reservation);
        Task<ReservationOperationResult> CreateReservationAsync(Reservation reservation);
        Task<List<Reservation>> GetReservationByGuest(string reservedBy);
    }
}
