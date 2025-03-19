namespace HotelProject.Domain.Model ;

public class BookingStatsByMonth
{
    public string Month { get; set; }
    public int TotalBookings { get; set; }
    public decimal Revenue { get; set; }
}