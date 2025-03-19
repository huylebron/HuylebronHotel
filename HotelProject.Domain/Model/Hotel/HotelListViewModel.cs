namespace HotelProject.Domain.Model.Hotel ;

public class HotelListViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string? ImageUrl { get; set; } // Lấy ảnh đầu tiên từ ImageJson
    public int TotalRooms { get; set; } // Tổng số phòng
    public int AvailableRooms { get; set; } // Số phòng còn trống
    
}