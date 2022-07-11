using Application.Reservations;
using Application.Reservations.Services;
using Domain.Reservations;
using Infrastructure;
using Infrastructure.Repositories.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tests
{
    [TestClass]
    public class ReservationTests
    {
        private readonly IReservationService _reservationService;
        private readonly IReservationRepository _reservationRepository;

        /// <summary>
        /// Sets up an in-memory database and configure the Reservation Service and Repository to be used for running the tests
        /// </summary>
        public ReservationTests()
        {
            var contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("ReservationServicesTests")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            DatabaseContext databaseContext = new(contextOptions);
            _reservationRepository = new ReservationRepository(databaseContext);
            _reservationService = new ReservationService(_reservationRepository);
        }

        /// <summary>
        /// Creating a new reservation. Should not return any errors and pass all validations
        /// </summary>
        [TestMethod]
        public async Task CreateReservation_Success()
        {
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(5), "John Doe");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(reservation);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Id);
            Assert.IsNull(result.Errors);
        }

        /// <summary>
        /// Attempting to create a reservation more than 30 days in advance. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateReservation_Failure_MoreThan30DaysInAdvance()
        {
            Reservation reservation = new(DateTime.Now.AddDays(31), DateTime.Now.AddDays(32), "Jane Doe");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(reservation);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Errors);
        }

        /// <summary>
        /// Attempting to create a reservation which lasts more than 3 days. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateReservation_Failure_MoreThan3DaysOfStay()
        {
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(6), "Joao Silva");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(reservation);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Errors);
        }

        /// <summary>
        /// Attempting to create a reservation with an ending date prior to the starting date. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateReservation_Failure_InvalidDates()
        {
            Reservation reservation = new(DateTime.Now.AddDays(4), DateTime.Now.AddDays(2), "Jean Dupont");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(reservation);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Errors);
        }

        /// <summary>
        /// Attempting to create a reservation without without informing reserved by. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateReservation_Failure_NoReservedBy()
        {
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), "");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(reservation);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Errors);
        }

        /// <summary>
        /// Attempting to create a reservation starting from the same day. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateReservation_Failure_StartingSameDay()
        {
            Reservation reservation = new(DateTime.Now, DateTime.Now.AddDays(2), "Jeanne Doeson");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(reservation);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Errors);
        }

        /// <summary>
        /// Attempting to create a second reservation for a guest not consecutively. Should be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateReservation_Success_Not3DaysConsecutively()
        {
            //Creating a valid reservation
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(5), "João Souza");
            ReservationOperationResult result1 = await _reservationService.CreateReservationAsync(reservation);

            long firstReservationId = result1.Data.Id;

            //Creating a second reservation, after 1 day have passed
            Reservation secondReservation = new(DateTime.Now.AddDays(7), DateTime.Now.AddDays(10), "João Souza");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(secondReservation);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreNotEqual(firstReservationId, result.Data.Id);
            Assert.IsNull(result.Errors);
        }

        /// <summary>
        /// Attempting to create a second reservation for a guest consecutively, resulting in more than 3 days straight. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateReservation_Failure_MoreThan3DaysConsecutively()
        {
            //Creating a valid reservation
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(5), "Jane Silvason");
            ReservationOperationResult result1 = await _reservationService.CreateReservationAsync(reservation);

            long firstReservationId = result1.Data.Id;

            //Creating a second reservation, right after the first
            Reservation secondReservation = new(DateTime.Now.AddDays(6), DateTime.Now.AddDays(9), "Jane Silvason");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(secondReservation);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Errors);
        }

        /// <summary>
        /// Canceling a reservation by informing its id. Should be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CancelReservation_Success()
        {
            //Creating a valid Reservation to be canceled
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), "Joana Silva");
            ReservationOperationResult result = await _reservationService.CreateReservationAsync(reservation);

            Assert.AreNotEqual(0, result.Data.Id);
            Assert.IsFalse(result.Data.Canceled);

            long canceledId = result.Data.Id;
            //Canceling the created reservation
            result = await _reservationService.CancelReservationAsync(result.Data.Id);
            Assert.IsTrue(result.Success);

            Reservation? canceledReservation = await _reservationRepository.GetByIdAsync(canceledId);

            Assert.IsNotNull(canceledReservation);
            Assert.IsTrue(canceledReservation.Canceled);
        }

        /// <summary>
        /// Attempting to cancel an inexisting reservation using a wrong id. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CancelReservation_Failure_NotFound()
        {
            //Passing an inexistent id as parameter
            ReservationOperationResult result = await _reservationService.CancelReservationAsync(99);            
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Errors);
        }

        /// <summary>
        /// Modifying a valid reservation with valid information
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ModifyReservation_Success()
        {
            //Creating a valid Reservation to be modified
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), "Jack Doe");
            ReservationOperationResult createResult = await _reservationService.CreateReservationAsync(reservation);

            long reservationId = createResult.Data.Id;

            //Data to update the reservation with
            var newReservationData = new Reservation(DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), "Jack Doe");
            ReservationOperationResult modifyResult = await _reservationService.ModifyReservationAsync(reservationId, newReservationData);

            //Asserting results of the modifying operation
            Assert.IsNotNull(modifyResult);
            Assert.IsTrue(modifyResult.Success);
            Assert.IsNull(modifyResult.Errors);
            Assert.IsNotNull(modifyResult.Data);

            //Attempting to retrieve the modified reservation and check the persisted data
            var persistedReservation = await _reservationRepository.GetByIdAsync(reservationId);
            Assert.IsNotNull(persistedReservation);
            Assert.AreEqual(newReservationData.StartDate, persistedReservation.StartDate);
            Assert.AreEqual(newReservationData.EndDate, persistedReservation.EndDate);
            Assert.AreEqual(newReservationData.ReservedBy, persistedReservation.ReservedBy);
        }

        /// <summary>
        /// Attempting to modify a reservation with invalid dates. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ModifyReservation_Failure_InvalidDates()
        {
            //Creating a valid Reservation to be modified
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), "Johnatan Silva");
            ReservationOperationResult createResult = await _reservationService.CreateReservationAsync(reservation);

            long reservationId = createResult.Data.Id;

            //Invalid data to try to update the reservation with 
            var invalidDates = new Reservation(DateTime.Now.AddDays(31), DateTime.Now.AddDays(34), "Johnatan Silva");
            ReservationOperationResult modifyResult = await _reservationService.ModifyReservationAsync(reservationId, invalidDates);

            Assert.IsNotNull(modifyResult);
            Assert.IsFalse(modifyResult.Success);
            Assert.IsNotNull(modifyResult.Errors);
        }

        /// <summary>
        /// Attempting to modify a reservation with a different value to ReservedBy property. Should not be valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ModifyReservation_Failure_DifferentGuest()
        {
            //Creating a valid Reservation to be modified
            Reservation reservation = new(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), "Johnatan Silva");
            ReservationOperationResult createResult = await _reservationService.CreateReservationAsync(reservation);

            long reservationId = createResult.Data.Id;

            //Invalid data to try to update the reservation with 
            var invalidDates = new Reservation(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), "Jim Silvason");
            ReservationOperationResult modifyResult = await _reservationService.ModifyReservationAsync(reservationId, invalidDates);

            Assert.IsNotNull(modifyResult);
            Assert.IsFalse(modifyResult.Success);
            Assert.IsNotNull(modifyResult.Errors);
        }

        /// <summary>
        /// Checking if the room is available when there are no reservations for the given period. Should be available
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CheckRoomAvailability_Available()
        {
            bool result = await _reservationService.CheckRoomAvailabilityAsync(DateTime.Now.AddDays(3), DateTime.Now.AddDays(5));
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checking if the room is available when there are reservations already placed for the given period. Should not be available
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CheckRoomAvailability_NotAvailable()
        {
            //Creating a valid Reservation
            Reservation reservation = new(DateTime.Now.AddDays(4), DateTime.Now.AddDays(6), "Jean Pierre");
            await _reservationService.CreateReservationAsync(reservation);

            //check if there are reservations within the entire period
            bool result = await _reservationService.CheckRoomAvailabilityAsync(DateTime.Now.AddDays(1), DateTime.Now.AddDays(10));
            Assert.IsFalse(result);
        }
    }
}