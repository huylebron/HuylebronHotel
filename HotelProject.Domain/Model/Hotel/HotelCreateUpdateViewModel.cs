using Microsoft . AspNetCore . Http ;

namespace HotelProject.Domain.Model.Hotel ;

public class HotelCreateUpdateViewModel
{
   
    
    public string Name { get; set; }
    
    
    public string? Description { get; set; }
    
  
    public string Address { get; set; }
    
    public List<IFormFile>? ImageFiles { get; set; } // Upload nhiều file ảnh
}