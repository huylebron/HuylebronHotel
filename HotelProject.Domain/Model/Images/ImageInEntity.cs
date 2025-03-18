namespace HotelProject.Domain.Model.Images ;

public class ImageInEntity
{
    public Guid ImageId { get; set; }
    public string ImageName { get; set; }
    public string ImageUrl { get; set; }

    public ImageInEntity(Guid imageId, string imageName, string imageUrl)
    {
        ImageId = imageId;
        ImageName = imageName;
        ImageUrl = imageUrl;
    }
}