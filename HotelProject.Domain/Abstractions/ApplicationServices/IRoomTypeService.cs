using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . RoomTypeViewModel ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IRoomTypeService
{
    Task<PageResult<RoomTypeViewModel>> GetRoomTypes(SearchQuery query);
    Task<RoomTypeViewModel> GetRoomTypeDetail(Guid roomTypeId);
    Task<ResponseResult> CreateRoomType(RoomTypeCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> UpdateRoomType(Guid roomTypeId, RoomTypeCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> DeleteRoomType(Guid roomTypeId);
    Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model);
}