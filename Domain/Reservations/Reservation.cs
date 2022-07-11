namespace Domain.Reservations
{
    public class Reservation : BaseEntity
    {
        public Reservation(DateTime startDate, DateTime endDate, string reservedBy)
        {
            StartDate = startDate;
            EndDate = endDate;
            ReservedBy = reservedBy;
            Canceled = false;
        }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The guest or client who places the reservation
        /// </summary>
        public string ReservedBy { get; set; }

        /// <summary>
        /// Flag for checking if the reservation has been canceled. When canceled, the reservation won't be returned in selecting requests.
        /// </summary>
        public bool Canceled { get; private set; }

        public void Cancel()
        {
            Canceled = true;
        }
    }
}
