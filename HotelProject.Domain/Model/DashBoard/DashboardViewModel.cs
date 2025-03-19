namespace HotelProject.Domain.Model ;

public class DashboardViewModel
{
    public int TotalHotels { get; set; }
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public List<BookingStatsByMonth> BookingStats { get; set; }
    public List<RoomOccupancyRate> RoomOccupancyRates { get; set; }
}