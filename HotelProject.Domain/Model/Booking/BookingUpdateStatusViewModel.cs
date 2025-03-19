using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Model.Booking ;

public class BookingUpdateStatusViewModel
{
    public Guid Id { get; set; }
    
  
    public BookingStatus BookingStatus { get; set; }
    
   
    public PaymentStatus PaymentStatus { get; set; }
    
    public List<Guid>? BookingStatusOptions { get; set; }
    public List<Guid>? PaymentStatusOptions { get; set; }
}