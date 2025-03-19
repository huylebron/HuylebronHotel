namespace HotelProject.Domain.Model.Hotel ;

public class HotelDetailViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Address { get; set; }
    public List<string>? Images { get; set; } // Chuyển đổi từ ImageJson
    public List<RoomTypeViewModel . RoomTypeViewModel> RoomTypes { get; set; }
}