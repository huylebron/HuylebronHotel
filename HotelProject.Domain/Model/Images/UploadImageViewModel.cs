using System . ComponentModel . DataAnnotations ;
using Microsoft . AspNetCore . Http ;

namespace HotelProject.Domain.Model.Images ;

public class UploadImageViewModel
{
    
    [Required]
    public List<IFormFile> Images { get; set; }
}