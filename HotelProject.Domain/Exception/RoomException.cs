namespace HotelProject.Domain.Exception ;

public static  class RoomException
{
    
    public class RoomNotFoundException : NotFoundException
    {
        public RoomNotFoundException(Guid roomId) 
            : base($"Không tìm thấy phòng với ID: {roomId}")
        {
        }
    }

    public class UpdateRoomException : BadRequestException
    {
        public UpdateRoomException(Guid roomId)
            : base($"Có lỗi xảy ra khi cập nhật phòng với ID: {roomId}")
        {
        }
    }

    public class DeleteRoomException : BadRequestException
    {
        public DeleteRoomException(Guid roomId)
            : base($"Có lỗi xảy ra khi xóa phòng với ID: {roomId}")
        {
        }
    }

    public class RoomNotAvailableException : BadRequestException
    {
        public RoomNotAvailableException(Guid roomId)
            : base($"Phòng với ID: {roomId} hiện không khả dụng")
        {
        }
    }
}