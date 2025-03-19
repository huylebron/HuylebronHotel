namespace HotelProject.Domain.Model.RoomTypeViewModel ;

public class RoomTypeViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; } // 
    public int TotalRooms { get; set; } // Tổng số phòng thuộc loại này
    
}