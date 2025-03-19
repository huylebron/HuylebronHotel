using HotelProject . Domain . Enum ;
using Microsoft . AspNetCore . Http ;

namespace HotelProject.Domain.Model.Room ;

public class RoomCreateUpdateViewModel 
{
   
    public string RoomNumber { get; set; }
   
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
  
    public int MaxOccupancy { get; set; }
    
  
    public double AreaInSquareMeters { get; set; }
    
   
  
    public decimal PricePerNight { get; set; }
    
  
    public decimal? DiscountPrice { get; set; }
    
   
    public Guid RoomTypeId { get; set; }
    
    public RoomStatus RoomStatus { get; set; }
    
    public List<IFormFile>? ImageFiles { get; set; } // Upload nhiều file ảnh
    
    public List<Guid>? AmenityIds { get; set; } // Danh sách ID các tiện nghi
    
    public List<Guid>? RoomTypeOptions { get; set; } // Danh sách loại phòng để chọn
    public List<Guid>? AmenityOptions { get; set; } // Danh sách tiện nghi để chọn
    public List<Guid>? RoomStatusOptions { get; set; } // Danh sách trạng thái phòng
}