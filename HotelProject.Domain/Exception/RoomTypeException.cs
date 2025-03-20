namespace HotelProject.Domain.Exception ;

public static class RoomTypeException
{
    public class RoomTypeNotFoundException : NotFoundException
    {
        public RoomTypeNotFoundException(Guid roomTypeId) 
            : base($"not found room type with : {roomTypeId}")
        {
        }
    }

    public class UpdateRoomTypeException : BadRequestException
    {
        public UpdateRoomTypeException(Guid roomTypeId)
            : base($"Có lỗi xảy ra khi cập nhật loại phòng với ID: {roomTypeId}")
        {
        }   
    }

    public class DeleteRoomTypeException : BadRequestException
    {
        public DeleteRoomTypeException(Guid roomTypeId)
            : base($"Có lỗi xảy ra khi xóa loại phòng với ID: {roomTypeId}")
        {
        }
    }
}