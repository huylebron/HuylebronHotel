namespace HotelProject.Domain.Exception ;

public static class HotelException
{
 
    public class HotelNotFoundException : NotFoundException
    {
        public HotelNotFoundException(Guid hotelId) 
            : base($"not found hotel with  ID: {hotelId}")
        {
        }
    }

    public class UpdateHotelException : BadRequestException
    {
        public UpdateHotelException(Guid hotelId)
            : base($"lỗi khi update hotel với ID: {hotelId}")
        {
        }
    }  

    public class DeleteHotelException : BadRequestException
    {
        public DeleteHotelException(Guid hotelId)
            : base($"lỗi khi delete hotel  với ID: {hotelId}")
        {
        }
    }
}