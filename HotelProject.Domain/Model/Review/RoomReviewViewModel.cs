namespace HotelProject.Domain.Model.Review ;

public class RoomReviewViewModel
{
    public Guid Id { get; set; }
    public string ReviewerName { get; set; }
    public string? Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedDate { get; set; }
}