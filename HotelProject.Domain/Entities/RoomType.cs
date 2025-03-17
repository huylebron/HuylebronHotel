using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;

[Table("RoomTypes")]
public class RoomType :DomainEntity<Guid>, IAuditTable
{
    [Column(TypeName = "nvarchar(500)")]
    public string Name { get; set; }
    
    [Column(TypeName = "nVarchar(500)")]
    public string? Description { get; set; }
    
    public string? ImageJson { get; set; }
    
    public Guid HotelId { get; set; }
    
    [ForeignKey(nameof(HotelId))]
    public Hotel Hotel { get; set; }
    
    public ICollection<Room> Rooms { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus Status { get; set; }
    
}