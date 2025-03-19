using HotelProject . Domain . Enum ;
using HotelProject . Domain . Model . AdditionalService ;
using HotelProject . Domain . Model . Room ;

namespace HotelProject.Domain.Model.Booking ;

public class BookingCreateViewModel
{
    public Guid UserId { get; set; } // Có thể lấy từ User hiện tại
    
  
    public Guid RoomId { get; set; }
    
  
    public DateTime CheckInDate { get; set; }
    
 
    public DateTime CheckOutDate { get; set; }
    
   
    public int NumberOfGuests { get; set; }
   
    public string? SpecialRequests { get; set; }
   
    public PaymentMethod PaymentMethod { get; set; }
    
    public List<Guid>? ServiceIds { get; set; } // Các dịch vụ bổ sung
    
    // Thông tin bổ sung để hiển thị
    public RoomDetailViewModel? Room { get; set; }
    public List<Guid>? RoomOptions { get; set; }
    public List<Guid>? PaymentMethodOptions { get; set; }
    public List<AdditionalServiceViewModel>? AvailableServices { get; set; }
}