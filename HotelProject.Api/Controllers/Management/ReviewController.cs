using HotelProject . Api . Controllers . Bases ;
using HotelProject . Api . Filters ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Review ;
using HotelProject . Domain . Utility ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject.Api.Controllers.Management ;

public class ReviewController : AuthorizeController
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

#region get

    [Permission(CommonConstants.Permissions.VIEW_REVIEW_PERMISSION)]
    [HttpPost]
    [Route("get-reviews")]
    public async Task<PageResult<RoomReviewViewModel>> GetReviews([FromBody] SearchQuery query)
    {
        var result = await _reviewService.GetReviews(query);
        return result;
    }

#endregion

#region create

    [Permission(CommonConstants.Permissions.ADD_REVIEW_PERMISSION)]
    [HttpPost]
    [Route("create-review")]
    public async Task<ResponseResult> CreateReview([FromBody] RoomReviewCreateViewModel model)
    {
        // Gán userId từ người dùng hiện tại
        model.UserId = CurrentUser.UserId;
        var result = await _reviewService.CreateReview(model, CurrentUser);
        return result;
    }

#endregion

#region delete

    [HttpDelete]
    [Route("delete-review")]
    public async Task<ResponseResult> DeleteReview(Guid reviewId)
    {
        var result = await _reviewService.DeleteReview(reviewId, CurrentUser);
        return result;
    }

    [Permission(CommonConstants.Permissions.UPDATE_REVIEW_PERMISSION)]
    [HttpPut]
    [Route("status-review")]
    public async Task<ResponseResult> UpdateStatus([FromBody] UpdateStatusViewModel model)
    {
        var result = await _reviewService.UpdateStatus(model);
        return result;
    }

#endregion
    
}