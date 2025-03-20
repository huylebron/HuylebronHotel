namespace HotelProject.Domain.Exception ;

public static class BookingException
{
    public class BookingNotFoundException : NotFoundException
    {
        public BookingNotFoundException(Guid bookingId) 
            : base($"Không tìm thấy đơn đặt phòng với ID: {bookingId}")
        {
        }
    }

    public class CreateBookingException : BadRequestException
    {
        public CreateBookingException()
            : base("Có lỗi xảy ra khi tạo đơn đặt phòng")
        {
        }
    }

    public class UpdateBookingException : BadRequestException
    {
        public UpdateBookingException(Guid bookingId)
            : base($"Có lỗi xảy ra khi cập nhật đơn đặt phòng với ID: {bookingId}")
        {
        }
    }

    public class InvalidDateRangeException : BadRequestException
    {
        public InvalidDateRangeException()
            : base("Ngày check-in phải trước ngày check-out")
        {
        }
    }

    public class RoomAlreadyBookedException : BadRequestException
    {
        public RoomAlreadyBookedException(Guid roomId)
            : base($"Phòng với ID: {roomId} đã được đặt trong khoảng thời gian này")
        {
        }
    }
    
    
}