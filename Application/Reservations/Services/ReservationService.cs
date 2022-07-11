using Domain.Reservations;
using FluentValidation.Results;
using Infrastructure.Repositories.Reservations;

namespace Application.Reservations.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        /// <summary>
        /// Get a specific reservation by id
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Reservation?> GetReservationAsync(long reservationId)
        {
            Reservation? reservation = await _reservationRepository.GetByIdAsync(reservationId);
            return reservation;
        }

        /// <summary>
        /// Cancel a reservation by Id
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        public async Task<ReservationOperationResult> CancelReservationAsync(long reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null)
                return new ReservationOperationResult(new string[] { "No reservation found for this identifier" });

            reservation.Cancel();
            await _reservationRepository.UpdateAsync(reservationId, reservation);

            return new ReservationOperationResult(true);
        }

        /// <summary>
        /// Create new reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        public async Task<ReservationOperationResult> CreateReservationAsync(Reservation reservation)
        {
            //Verify if there are any reservations between the informed dates
            if (!await CheckRoomAvailabilityAsync(reservation.StartDate, reservation.EndDate))
                return new ReservationOperationResult(new string[] { "The selected period is not available." });

            //Check if the reservation object is valid
            var validator = new ReservationValidator();
            ValidationResult validationResult = validator.Validate(reservation);

            if (validationResult.IsValid)
            {
                if (!await ValidateGuestCurrentReservations(reservation.StartDate, reservation.EndDate, reservation.ReservedBy))
                        return new ReservationOperationResult(new string[] { "A guest can not reserve more than 3 days straight." });

                Reservation created = await _reservationRepository.CreateAsync(reservation);
                return new ReservationOperationResult(created);
            }

            return new ReservationOperationResult(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        /// <summary>
        /// Returns all canceled reservations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Reservation>> GetCancelledReservationsAsync()
            => await _reservationRepository.GetCanceledAsync();

        /// <summary>
        /// Change dates of reservation
        /// </summary>
        /// <param name="reservationId"></param>
        /// <param name="reservation"></param>
        /// <returns></returns>
        public async Task<ReservationOperationResult> ModifyReservationAsync(long reservationId, Reservation reservation)
        {
            //Check if the reservation object is valid
            var validator = new ReservationValidator();
            ValidationResult validationResult = validator.Validate(reservation);

            if (validationResult.IsValid)
            {
                var reservationToUpdate = await _reservationRepository.GetByIdAsync(reservationId);

                if (reservationToUpdate == null)
                    return new ReservationOperationResult(new string[] { "No reservation found for this identifier." });

                if (reservation.ReservedBy != reservationToUpdate.ReservedBy)
                    return new ReservationOperationResult(new string[] { "Changing the placer of the reservation is not allowed." });

                reservationToUpdate.StartDate = reservation.StartDate;
                reservationToUpdate.EndDate = reservation.EndDate;
                reservationToUpdate.ReservedBy = reservation.ReservedBy;

                reservationToUpdate = await _reservationRepository.UpdateAsync(reservationId, reservationToUpdate);
                return new ReservationOperationResult(reservationToUpdate);
            }

            return new ReservationOperationResult(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        /// <summary>
        /// Returns all current not cancelled reservations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Reservation>> GetReservationsAsync()
        {
            List<Reservation> reservations = await _reservationRepository.GetCurrentReservationsAsync();
            return reservations;
        }

        /// <summary>
        /// Check if room is available to book during the given dates. Returns false if there are already registered reservations
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<bool> CheckRoomAvailabilityAsync(DateTime startDate, DateTime endDate)
        {
            bool existingReservations = await _reservationRepository.AreReservationsInPeriodAsync(startDate, endDate);
            return !existingReservations;
        }

        /// <summary>
        /// Returns all current reservations of specified guest
        /// </summary>
        /// <param name="reservedBy"></param>
        /// <returns></returns>
        public async Task<List<Reservation>> GetReservationByGuest(string reservedBy)
        {
            List<Reservation> reservations = await _reservationRepository.GetReservationsByGuest(reservedBy);
            return reservations;
        }

        /// <summary>
        /// Check if a specified guest is able to place a reservation, considering their current reservations' dates
        /// </summary>
        /// <param name="startDate">The starting date for the new reservation</param>
        /// <param name="endDate">The ending date for the new reservation</param>
        /// <param name="guestName">The guest to whom the reservation is placed</param>
        /// <returns>Return true if the reservation can be recorded. Otherwise, returns false.</returns>
        private async Task<bool> ValidateGuestCurrentReservations(DateTime startDate, DateTime endDate, string guestName) 
        {
            List<Reservation> guestReservations = await _reservationRepository.GetReservationsByGuest(guestName);

            if (guestReservations.Count == 0)
                return true;

            int daysToReserve = (endDate.Date - startDate.Date).Days;

            foreach (var reservation in guestReservations)
            {
                //Check if there is a reservation up to one day prior (consecutive days)
                if (startDate.Date.AddDays(-1) == reservation.EndDate.Date || endDate.Date.AddDays(1) == reservation.StartDate.Date)
                {
                    //Must not allow the guest to make more than a 3 days stay
                    int reservedDays = (reservation.EndDate.Date - reservation.StartDate.Date).Days;
                    if (daysToReserve + reservedDays > 3)
                        return false;
                }
            }

            return true;
        }
    }
}
