namespace HotelProject.Domain.Exception ;

public static class ReviewException
{
 
    
} public class ReviewNotFoundException : NotFoundException
{
    public ReviewNotFoundException(Guid reviewId) 
        : base($"Không tìm thấy đánh giá với ID: {reviewId}")
    {
    }
}

public class CreateReviewException : BadRequestException
{
    public CreateReviewException()
        : base("Có lỗi xảy ra khi tạo đánh giá")
    {
    }
}

public class DeleteReviewException : BadRequestException
{
    public DeleteReviewException(Guid reviewId)
        : base($"Có lỗi xảy ra khi xóa đánh giá với ID: {reviewId}")
    {
    }
}

public class UnauthorizedReviewException : BadRequestException
{
    public UnauthorizedReviewException()
        : base("Bạn không có quyền thao tác trên đánh giá này")
    {
    }
}