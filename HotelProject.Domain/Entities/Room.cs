using System . ComponentModel . DataAnnotations . Schema ;
using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Entities ;
[Table("Rooms")]
public class Room : DomainEntity<Guid>, IAuditTable
{
    [Column(TypeName = "nvarchar(200)")]
    public string RoomNumber { get; set; } // Số phòng
    
    [Column(TypeName = "nvarchar(500)")]
    public string Name { get; set; }
    
    [Column(TypeName = "ntext")]
    public string? Description { get; set; }
    
    public int MaxOccupancy { get; set; } // Số người ở tối đa
    
    public double AreaInSquareMeters { get; set; } // Diện tích phòng
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerNight { get; set; } // Giá theo đêm
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPrice { get; set; } // Giá khuyến mãi 
    
    public string? ImageJson { get; set; } // Hình ảnh phòng    
    
    public Guid RoomTypeId { get; set; }
    
    [ForeignKey(nameof(RoomTypeId))]
    public RoomType RoomType { get; set; }
    
    public RoomStatus RoomStatus { get; set; } // Trạng thái phòng
    
    public ICollection<RoomAmenity> RoomAmenities { get; set; }
    
    public ICollection<RoomReview> Reviews { get; set; }
    
    public ICollection<Booking> Bookings { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus Status { get; set; }
    
}