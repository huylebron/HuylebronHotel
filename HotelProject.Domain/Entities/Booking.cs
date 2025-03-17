using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;
[Table("Bookings")]
public class Booking  : DomainEntity<Guid>, IAuditTable
{
    public Guid UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; }
    
    public Guid RoomId { get; set; }
    
    [ForeignKey(nameof(RoomId))]
    public Room Room { get; set; }
    
    public DateTime CheckInDate { get; set; }
    
    public DateTime CheckOutDate { get; set; }
    
    public int NumberOfGuests { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    [Column(TypeName = "nvarchar(1000)")]
    public string? SpecialRequests { get; set; }
    
    public BookingStatus BookingStatus { get; set; }
    
    public PaymentMethod PaymentMethod { get; set; }
    
    public PaymentStatus PaymentStatus { get; set; }
    
    public ICollection<BookingService>? BookingServices { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus Status { get; set; }
}