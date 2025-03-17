using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;
[Table("AdditionalServices")]
public class AdditionalService : DomainEntity<Guid>, IAuditTable
{
    [Column(TypeName = "nvarchar(500)")]
    public string Name { get; set; } // Tên dịch vụ 
    
    [Column(TypeName = "ntext")]
    public string? Description { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    public string? ImageJson { get; set; }
    
    public ICollection<BookingService> BookingServices { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus Status { get; set; }
    
}