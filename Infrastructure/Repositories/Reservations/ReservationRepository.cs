using Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Reservations
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(DatabaseContext databaseContext) : base(databaseContext)
        { }

        /// <summary>
        /// Returns the current valid reservations, which are not canceled or finished
        /// </summary>
        /// <returns></returns>
        public async Task<List<Reservation>> GetCurrentReservationsAsync()
        {
            var result = await databaseContext.Reservations.Where(r => !r.Canceled && r.EndDate.Date > DateTime.Now).ToListAsync();
            return result;
        }

        /// <summary>
        /// Returns all canceled reservations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Reservation>> GetCanceledAsync()
        {
            var result = await databaseContext.Reservations.Where(r => r.Canceled).ToListAsync();
            return result;
        }

        /// <summary>
        /// Check if there is a reservation between given starting and ending dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<Reservation>> GetReservationsByPeriodAsync(DateTime startDate, DateTime endDate)
        { 
            var reservations = await databaseContext.Reservations.Where(r => r.StartDate.Date >= startDate.Date && r.EndDate.Date <= endDate.Date).ToListAsync();
            return reservations;
        }

        /// <summary>
        /// Returns all current reservations of a specified guest
        /// </summary>
        /// <param name="guestName"></param>
        /// <returns></returns>
        public async Task<List<Reservation>> GetReservationsByGuest(string guestName)
        { 
            var result = await databaseContext.Reservations.Where(r => r.ReservedBy == guestName && !r.Canceled).ToListAsync();
            return result;
        }
    }
}
