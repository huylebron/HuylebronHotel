namespace HotelProject.Domain.Exception ;

public static class AdditionalServiceException
{
    public class ServiceNotFoundException : NotFoundException
    {
        public ServiceNotFoundException(Guid serviceId) 
            : base($"Không tìm thấy dịch vụ với ID: {serviceId}")
        {
        }
    }

    public class UpdateServiceException : BadRequestException
    {
        public UpdateServiceException(Guid serviceId)
            : base($"Có lỗi xảy ra khi cập nhật dịch vụ với ID: {serviceId}")
        {
        }
    }

    public class DeleteServiceException : BadRequestException
    {
        public DeleteServiceException(Guid serviceId)
            : base($"Có lỗi xảy ra khi xóa dịch vụ với ID: {serviceId}")
        {
        }
    }
}