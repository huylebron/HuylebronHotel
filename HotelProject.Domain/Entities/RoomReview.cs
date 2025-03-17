using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;
[Table("RoomReviews")]
public class RoomReview : DomainEntity<Guid>, IAuditTable
{
    public Guid UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; }
    
    [Column(TypeName = "nvarchar(500)")]
    public string ReviewerName { get; set; }
    
    [Column(TypeName = "ntext")]
    public string? Content { get; set; }
    
    public int Rating { get; set; }
    
    public Guid RoomId { get; set; }
        
    [ForeignKey(nameof(RoomId))]
    public Room Room { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus Status { get; set; }
}