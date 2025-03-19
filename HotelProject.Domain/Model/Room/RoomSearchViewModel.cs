namespace HotelProject.Domain.Model.Room ;

public class RoomSearchViewModel
{
    public Guid? HotelId { get; set; }
    public Guid? RoomTypeId { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? Guests { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<Guid>? AmenityIds { get; set; }
    
    public List<Guid>? HotelOptions { get; set; }
    public List<Guid>? RoomTypeOptions { get; set; }
    public List<Guid>? AmenityOptions { get; set; }
    
    public List<RoomListViewModel>? SearchResults { get; set; }
}