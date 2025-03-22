using HotelProject . Api . Controllers . Bases ;
using HotelProject . Api . Filters ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . RoomTypeViewModel ;
using HotelProject . Domain . Utility ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject.Api.Controllers.Management ;

[Route("api/[controller]")]
[ApiController]
public class RoomTypeController : AuthorizeController
{
    
     private readonly IRoomTypeService _roomTypeService;

    public RoomTypeController(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    #region get

    [HttpPost]
    [Route("get-room-types")]
    public async Task<PageResult<RoomTypeViewModel>> GetRoomTypes([FromBody] SearchQuery query)
    {
        var result = await _roomTypeService.GetRoomTypes(query);
        return result;
    }

    #endregion


    #region detail

    [HttpGet]
    [Route("detail-room-type")]
    public async Task<RoomTypeViewModel> GetRoomTypeDetail(Guid roomTypeId)
    {
        var result = await _roomTypeService.GetRoomTypeDetail(roomTypeId);
        return result;
    }

    #endregion


    #region create

    [Permission(CommonConstants.Permissions.ADD_ROOMTYPE_PERMISSION)]
    [HttpPost]
    [Route("create-room-type")]
    public async Task<ResponseResult> CreateRoomType([FromForm] RoomTypeCreateUpdateViewModel model)
    {
        var result = await _roomTypeService.CreateRoomType(model, CurrentUser);
        return result;
    }

    #endregion


    #region update

    [Permission(CommonConstants.Permissions.UPDATE_ROOMTYPE_PERMISSION)]
    [HttpPut]
    [Route("update-room-type")]
    public async Task<ResponseResult> UpdateRoomType(Guid roomTypeId, [FromForm] RoomTypeCreateUpdateViewModel model)
    {
        var result = await _roomTypeService.UpdateRoomType(roomTypeId, model, CurrentUser);
        return result;
    }

    #endregion


    #region delete

    [Permission(CommonConstants.Permissions.DELETE_ROOMTYPE_PERMISSION)]
    [HttpDelete]
    [Route("delete-room-type")]
    public async Task<ResponseResult> DeleteRoomType(Guid roomTypeId)
    {
        var result = await _roomTypeService.DeleteRoomType(roomTypeId);
        return result;
    }

    [Permission(CommonConstants.Permissions.UPDATE_ROOMTYPE_PERMISSION)]
    [HttpPut]
    [Route("status-room-type")]
    public async Task<ResponseResult> UpdateStatus([FromBody] UpdateStatusViewModel model)
    {
        var result = await _roomTypeService.UpdateStatus(model);
        return result;
    }

    #endregion
    
    
    }
