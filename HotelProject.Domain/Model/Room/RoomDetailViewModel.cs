using HotelProject . Domain . Enum ;
using HotelProject . Domain . Model . Amenity ;
using HotelProject . Domain . Model . Review ;

namespace HotelProject.Domain.Model.Room ;

public class RoomDetailViewModel
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MaxOccupancy { get; set; }
    public double AreaInSquareMeters { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal ActualPrice => DiscountPrice ?? PricePerNight;
    public List<string>? Images { get; set; } // Chuyển đổi từ ImageJson
    public string RoomTypeName { get; set; }
    public Guid RoomTypeId { get; set; }
    public string HotelName { get; set; }
    public RoomStatus RoomStatus { get; set; }
    public string RoomStatusName { get; set; }
    public List<AmenityViewModel> Amenities { get; set; }
    public List<RoomReviewViewModel> Reviews { get; set; }
}