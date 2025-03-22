using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Review ;
using HotelProject . Domain . Model . Users ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Logging ;

namespace HotelProject . Application . Services ;

public class ReviewService : IReviewService
{
    private readonly IGenericRepository < Booking , Guid > _bookingRepository ;
    private readonly ILogger < ReviewService > _logger ;
    private readonly IGenericRepository < RoomReview , Guid > _reviewRepository ;
    private readonly IGenericRepository < Room , Guid > _roomRepository ;
    private readonly IUnitOfWork _unitOfWork ;

    public async Task < PageResult < RoomReviewViewModel > > GetReviews ( SearchQuery query ) {
        var result = new PageResult < RoomReviewViewModel >
        {
            CurrentPage = query . PageIndex
        } ;

        var reviewQuery = _reviewRepository . FindAll ( includeProperties : r => r . User ) ;

        // Lọc theo trạng thái nếu cần
        if ( query . DisplayActiveItem )
            reviewQuery = reviewQuery . Where ( s => s . Status == EntityStatus . Active ) ;

        // Lọc theo từ khóa nếu có
        if ( ! string . IsNullOrEmpty ( query . Keyword ) )
            reviewQuery = reviewQuery . Where ( s =>
                s . ReviewerName . Contains ( query . Keyword ) ||
                ( s . Content != null && s . Content . Contains ( query . Keyword ) ) ) ;

        // Đếm tổng số bản ghi thỏa mãn điều kiện query
        result . TotalCount = await reviewQuery . CountAsync ( ) ;

        // Lấy dữ liệu phân trang
        var reviews = await reviewQuery
                            . OrderByDescending ( s => s . CreatedDate )
                            . Skip ( query . SkipNo )
                            . Take ( query . TakeNo )
                            . Select ( s => new RoomReviewViewModel
                            {
                                Id = s . Id ,
                                ReviewerName = s . ReviewerName ,
                                Content = s . Content ,
                                Rating = s . Rating ,
                                CreatedDate = s . CreatedDate ?? DateTime . MinValue
                            } )
                            . ToListAsync ( ) ;

        result . Data = reviews ;
        return result ;
    }

    public async Task < ResponseResult >
        CreateReview ( RoomReviewCreateViewModel model , UserProfileModel currentUser ) {
        // Kiểm tra phòng tồn tại
        var room = await _roomRepository . FindByIdAsync ( model . RoomId ) ;
        if ( room == null ) throw new RoomException . RoomNotFoundException ( model . RoomId ) ;

        // Kiểm tra người dùng đã từng đặt phòng này chưa (tùy chọn)
        var hasBooking = await _bookingRepository . FindAll ( b =>
                                                      b . UserId == model . UserId &&
                                                      b . RoomId == model . RoomId &&
                                                      b . BookingStatus == BookingStatus . CheckedOut )
                                                  . AnyAsync ( ) ;

        if ( ! hasBooking && ! currentUser . Permissions . Contains ( "ADD_REVIEW_PERMISSION" ) )
            return ResponseResult . Fail ( " Không thêm đánh giá cho phòng nay" ) ;

        // Kiểm tra rating hợp lệ (1-5)
        if ( model . Rating < 1 || model . Rating > 5 )
            return ResponseResult . Fail ( "Rating phải trong khoảng 1-5" ) ;

        var newReview = new RoomReview
        {
            Id = Guid . NewGuid ( ) ,
            UserId = model . UserId ,
            ReviewerName = model . ReviewerName ,
            Content = model . Content ,
            Rating = model . Rating ,
            RoomId = model . RoomId ,
            CreatedBy = currentUser . UserId ,
            CreatedDate = DateTime . Now ,
            Status = EntityStatus . Active
        } ;

        try
        {
            _reviewRepository . Add ( newReview ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Đánh giá phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Lỗi khi tạo đánh giá phòng" ) ;
            throw new CreateReviewException ( ) ;
        }
    }

    public async Task < ResponseResult > DeleteReview ( Guid reviewId , UserProfileModel currentUser ) {
        var review = await _reviewRepository . FindByIdAsync ( reviewId ) ;
        if ( review == null ) throw new ReviewNotFoundException ( reviewId ) ;

        // Kiểm tra quyền: chỉ người tạo review hoặc admin có thể xóa
        if ( review . UserId != currentUser . UserId &&
             ! currentUser . Permissions . Contains ( "DELETE_REVIEW_PERMISSION" ) )
            throw new UnauthorizedReviewException ( ) ;

        try
        {
            _reviewRepository . Remove ( review ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Xóa đánh giá thành công" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Lỗi khi xóa đánh giá với ID: {ReviewId}" , reviewId ) ;
            throw new DeleteReviewException ( reviewId ) ;
        }
    }

    public async Task < ResponseResult > UpdateStatus ( UpdateStatusViewModel model ) {
        var review = await _reviewRepository . FindByIdAsync ( model . Id ) ;
        if ( review == null ) throw new ReviewNotFoundException ( model . Id ) ;

        review . Status = model . Status ;

        try
        {
            _reviewRepository . Update ( review ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Cập nhật trạng thái đánh giá thành công" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Lỗi khi cập nhật trạng thái đánh giá có ID: {ReviewId}" , model . Id ) ;
            throw new UnauthorizedReviewException ( ) ;
        }
    }

    public ReviewService (
        IGenericRepository < RoomReview , Guid > reviewRepository ,
        IGenericRepository < Room , Guid > roomRepository ,
        IGenericRepository < Booking , Guid > bookingRepository ,
        IUnitOfWork unitOfWork ,
        ILogger < ReviewService > logger ) {
        _reviewRepository = reviewRepository ;
        _roomRepository = roomRepository ;
        _bookingRepository = bookingRepository ;
        _unitOfWork = unitOfWork ;
        _logger = logger ;
    }
}