namespace HotelProject.Domain.Exception ;

public static class AmenityException
{
  
    public class AmenityNotFoundException : NotFoundException
    {
        public AmenityNotFoundException(Guid amenityId) 
            : base($"Không tìm thấy tiện nghi với ID: {amenityId}")
        {
        }
    }

    public class UpdateAmenityException : BadRequestException
    {
        public UpdateAmenityException(Guid amenityId)
            : base($"Có lỗi xảy ra khi cập nhật tiện nghi với ID: {amenityId}")
        {
        }
    }

    public class DeleteAmenityException : BadRequestException
    {
        public DeleteAmenityException(Guid amenityId)
            : base($"Có lỗi xảy ra khi xóa tiện nghi với ID: {amenityId}")
        {
        }
    }
}