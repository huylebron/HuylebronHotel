using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Model.Room ;

public class RoomListViewModel
{
    
    public Guid Id { get; set; }
    public string RoomNumber { get; set; }
    public string Name { get; set; }
    public int MaxOccupancy { get; set; }
    public double AreaInSquareMeters { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? ImageUrl { get; set; } // Lấy ảnh đầu tiên từ ImageJson
    public string RoomTypeName { get; set; }
    public string RoomStatusName { get; set; }
    public RoomStatus RoomStatus { get; set; }
    public bool IsAvailable => RoomStatus == RoomStatus.Available;
}