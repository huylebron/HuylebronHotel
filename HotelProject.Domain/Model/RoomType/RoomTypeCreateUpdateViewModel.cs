using Microsoft . AspNetCore . Http ;

namespace HotelProject.Domain.Model.RoomTypeViewModel ;

public class RoomTypeCreateUpdateViewModel
{
    public string ? Name { get; set; } 


    public string? Description { get; set; }
    
    
    public Guid HotelId { get; set; }
    
    public List<IFormFile>? ImageFiles { get; set; } // Upload nhiều file ảnh
    
  
}