using FluentValidation;

namespace Domain.Reservations
{
    /// <summary>
    /// Validates if the reservation is within the specified criteria
    /// </summary>
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        private const string ReservationNextDayMessage = "Reservations must be placed starting from the next day.";
        private const string InvalidPeriodMessage = "The period for the reservation is invalid";
        private const string ReservedByNotInformedMessage = "Must inform who is placing the reservation";
        private const string ThirtyDaysInAdvanceMessage = "Reservations must only be made up to 30 days in advance.";
        private const string ThreeDayLimitMessage = "Reservations must have a maximum period of 3 days";

        public ReservationValidator()
        {
            DateTime dateIn30days = DateTime.Now.AddDays(30);
            RuleFor(r => r.StartDate.Date).NotEqual(DateTime.Now.Date).WithMessage(ReservationNextDayMessage);
            RuleFor(r => r.StartDate.Date).LessThanOrEqualTo(r => r.EndDate.Date).WithMessage(InvalidPeriodMessage);
            RuleFor(r => r.StartDate).LessThanOrEqualTo(dateIn30days).WithMessage(ThirtyDaysInAdvanceMessage);            
            RuleFor(r => (r.EndDate - r.StartDate).Days).LessThanOrEqualTo(3).WithMessage(ThreeDayLimitMessage);
            RuleFor(r => r.ReservedBy).NotEmpty().NotNull().WithMessage(ReservedByNotInformedMessage);
        }
    }
}
