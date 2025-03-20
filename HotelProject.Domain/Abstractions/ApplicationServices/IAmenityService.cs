using HotelProject . Domain . Model . Amenity ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IAmenityService
{
    Task<PageResult<AmenityViewModel>> GetAmenities(SearchQuery query);
    Task<AmenityViewModel> GetAmenityDetail(Guid amenityId);
    Task<ResponseResult> CreateAmenity(AmenityCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> UpdateAmenity(Guid amenityId, AmenityCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> DeleteAmenity(Guid amenityId);
    Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model);
}