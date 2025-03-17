using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;
[Table("Amenities")]
public class Amenity : DomainEntity<Guid>, IAuditTable
{
    [Column(TypeName = "nvarchar(500)")]
    public string Name { get; set; } // Tên tiện nghi
    
    [Column(TypeName = "ntext")]
    public string? Description { get; set; }
    
   
    
    public ICollection<RoomAmenity> RoomAmenities { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus Status { get; set; }
}