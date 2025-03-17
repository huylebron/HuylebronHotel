using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;
[Table("BookingsServices")]
public class BookingService: DomainEntity<Guid>, IAuditTable
{
    public Guid BookingId { get; set; }
    
    [ForeignKey(nameof(BookingId))]
    public Booking Booking { get; set; }
    
    public Guid ServiceId { get; set; }
    
    [ForeignKey(nameof(ServiceId))]
    public AdditionalService Service { get; set; }
    
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus Status { get; set; }
    
}