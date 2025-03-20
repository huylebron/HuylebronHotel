using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Images ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IImageService
{
    Task<PageResult<ImageViewModel>> GetImages(ImageSearchQuery query);
    Task<ResponseResult> UploadImages(UploadImageViewModel model, UserProfileModel? currentUser = null);

    Task<ResponseResult> UpdateImage(UpdateImageViewModel model, UserProfileModel? currentUser = null);
    Task<ResponseResult> DeleteImage(Guid imageId);

    Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model);
}