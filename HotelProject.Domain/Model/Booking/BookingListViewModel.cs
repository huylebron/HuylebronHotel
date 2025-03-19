using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Model.Booking ;

public class BookingListViewModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string RoomName { get; set; }
    public string RoomNumber { get; set; }
    public string HotelName { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public string BookingStatusName { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentStatusName { get; set; }
}