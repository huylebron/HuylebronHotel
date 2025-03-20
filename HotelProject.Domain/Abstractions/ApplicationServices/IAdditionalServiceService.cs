using HotelProject . Domain . Model . AdditionalService ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IAdditionalServiceService
{
    Task<PageResult<AdditionalServiceViewModel>> GetServices(SearchQuery query);
    Task<AdditionalServiceViewModel> GetServiceDetail(Guid serviceId);
    Task<ResponseResult> CreateService(AdditionalServiceCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> UpdateService(Guid serviceId, AdditionalServiceCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> DeleteService(Guid serviceId);
    Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model);
}