using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Review ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IReviewService
{
    Task<PageResult<RoomReviewViewModel>> GetReviews(SearchQuery query);
    Task<ResponseResult> CreateReview(RoomReviewCreateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> DeleteReview(Guid reviewId, UserProfileModel currentUser);
    Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model);
}