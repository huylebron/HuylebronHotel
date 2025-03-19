using HotelProject . Domain . Enum ;
using HotelProject . Domain . Model . AdditionalService ;

namespace HotelProject.Domain.Model.Booking ;

public class BookingDetailViewModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string RoomName { get; set; }
    public string RoomNumber { get; set; }
    public string HotelName { get; set; }
    public string HotelAddress { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
    public int NumberOfGuests { get; set; }
    public decimal RoomPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string? SpecialRequests { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public string BookingStatusName { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentStatusName { get; set; }
    public List<BookingServiceViewModel> Services { get; set; }
}