using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Room ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IRoomService
{
    Task<PageResult<RoomListViewModel>> GetRooms(SearchQuery query);
    Task<RoomDetailViewModel> GetRoomDetail(Guid roomId);
    Task<ResponseResult> CreateRoom(RoomCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> UpdateRoom(Guid roomId, RoomCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> DeleteRoom(Guid roomId);
    Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model);
    Task<List<RoomListViewModel>> SearchAvailableRooms(RoomSearchViewModel searchModel);
}