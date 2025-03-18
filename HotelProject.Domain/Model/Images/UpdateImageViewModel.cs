using System . ComponentModel . DataAnnotations ;

namespace HotelProject.Domain.Model.Images ;

public class UpdateImageViewModel
{
    
    [Required]
    public Guid ImageId { get; set; }

    [Required]
    public string ImageName { get; set; }
}