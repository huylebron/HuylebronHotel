using Microsoft . AspNetCore . Http ;

namespace HotelProject.Domain.Model.AdditionalService ;

public class AdditionalServiceCreateUpdateViewModel
{
    
    public string Name { get; set; }
    
    
    public string? Description { get; set; }
    
 
    public decimal Price { get; set; }
    
    public IFormFile? ImageFile { get; set; } // Upload file ảnh
}