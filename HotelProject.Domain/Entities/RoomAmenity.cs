using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;
[Table("RoomAmenities")]
public class RoomAmenity : DomainEntity<Guid>
{
    public Guid RoomId { get; set; }
    
    [ForeignKey(nameof(RoomId))]
    public Room Room { get; set; }
    
    public Guid AmenityId { get; set; }
    
    [ForeignKey(nameof(AmenityId))]
    public Amenity Amenity { get; set; }
    
    
    
}