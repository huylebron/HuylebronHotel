using HotelProject . Api . Controllers . Bases ;
using HotelProject . Api . Filters ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Room ;
using HotelProject . Domain . Utility ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject.Api.Controllers.Management ;
[Route("api/[controller]")]
[ApiController]
public class RoomController : AuthorizeController
{
   private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    #region get

    [HttpPost]
    [Route("get-rooms")]
    public async Task<PageResult<RoomListViewModel>> GetRooms([FromBody] SearchQuery query)
    {
        var result = await _roomService.GetRooms(query);
        return result;
    }

    #endregion


    #region detail

    [HttpGet]
    [Route("detail-room")]
    public async Task<RoomDetailViewModel> GetRoomDetail(Guid roomId)
    {
        var result = await _roomService.GetRoomDetail(roomId);
        return result;
    }

    #endregion


    #region create

    [Permission(CommonConstants.Permissions.ADD_ROOM_PERMISSION)]
    [HttpPost]
    [Route("create-room")]
    public async Task<ResponseResult> CreateRoom([FromForm] RoomCreateUpdateViewModel model)
    {
        var result = await _roomService.CreateRoom(model, CurrentUser);
        return result;
    }

    #endregion


    #region update

    [Permission(CommonConstants.Permissions.UPDATE_ROOM_PERMISSION)]
    [HttpPut]
    [Route("update-room")]
    public async Task<ResponseResult> UpdateRoom(Guid roomId, [FromForm] RoomCreateUpdateViewModel model)
    {
        var result = await _roomService.UpdateRoom(roomId, model, CurrentUser);
        return result;
    }

    #endregion


    #region delete

    [Permission(CommonConstants.Permissions.DELETE_ROOM_PERMISSION)]
    [HttpDelete]
    [Route("delete-room")]
    public async Task<ResponseResult> DeleteRoom(Guid roomId)
    {
        var result = await _roomService.DeleteRoom(roomId);
        return result;
    }

    [Permission(CommonConstants.Permissions.UPDATE_ROOM_PERMISSION)]
    [HttpPut]
    [Route("status-room")]
    public async Task<ResponseResult> UpdateStatus([FromBody] UpdateStatusViewModel model)
    {
        var result = await _roomService.UpdateStatus(model);
        return result;
    }

    #endregion

    #region search

    [HttpPost]
    [Route("search-available-rooms")]
    public async Task<List<RoomListViewModel>> SearchAvailableRooms([FromBody] RoomSearchViewModel model)
    {
        var result = await _roomService.SearchAvailableRooms(model);
        return result;
    }

    #endregion
}