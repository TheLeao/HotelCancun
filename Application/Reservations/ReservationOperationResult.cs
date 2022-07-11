using Domain.Reservations;

namespace Application.Reservations
{
    public record ReservationOperationResult
    {
        public ReservationOperationResult(bool success)
        {
            Success = success;
        }

        public ReservationOperationResult(Reservation data)
        {
            Data = data;
            Success = true;
        }

        public ReservationOperationResult(string[] errors)
        {
            Errors = errors;
            Success = false;
        }

        public Reservation Data { get; set; }
        public string[] Errors { get; set; }
        public bool Success { get; set; }
    }
}
