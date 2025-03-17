using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;
[Table("Hotels")]
public class Hotel : DomainEntity<Guid>, IAuditTable
{
    [Column(TypeName = "nvarchar(500)")]
    public string Name { get; set; }
    
    [Column(TypeName = "nvarchar(500)")]
    public string? Description { get; set; }
    
    public string Address { get; set; }
    
    public string? ImageJson { get; set; }
    
   
    
    public ICollection<RoomType> RoomTypes { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus Status { get; set; }
    
    
    
    
}