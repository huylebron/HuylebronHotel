namespace HotelProject.Domain.Model.Review ;

public class RoomReviewCreateViewModel
{
    public Guid UserId { get; set; } // Có thể lấy từ User hiện tại
    
    public string ReviewerName { get; set; }
    
    public string? Content { get; set; }
    
   
    public int Rating { get; set; }
    
  
    public Guid RoomId { get; set; }
}