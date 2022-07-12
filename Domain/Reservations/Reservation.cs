namespace Domain.Reservations
{
    /// <summary>
    /// A reservation placed in the hotel's room for a specified period and guest
    /// </summary>
    public class Reservation : BaseEntity
    {
        public Reservation(DateTime startDate, DateTime endDate, string reservedBy)
        {
            StartDate = startDate;
            EndDate = endDate;
            ReservedBy = reservedBy;
            Canceled = false;
        }
        
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// The guest or client who places the reservation
        /// </summary>
        public string ReservedBy { get; private set; }

        /// <summary>
        /// Flag for checking if the reservation has been canceled. When canceled, the reservation won't be returned in selecting requests.
        /// </summary>
        public bool Canceled { get; private set; }

        /// <summary>
        /// Modify the values of this reservation's dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void UpdateDates(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Cancel this reservation
        /// </summary>
        public void Cancel()
        {
            Canceled = true;
        }
    }
}
