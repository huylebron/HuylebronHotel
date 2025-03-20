using HotelProject . Api . Controllers . Bases ;
using HotelProject . Api . Filters ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Hotel ;
using HotelProject . Domain . Utility ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject.Api.Controllers.Management ;

public class HotelController : AuthorizeController
{
    // GET
    
     private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpPost]
        [Route("get-hotels")]
        public async Task<PageResult<HotelListViewModel>> GetHotels([FromBody] SearchQuery query)
        {
            var result = await _hotelService.GetHotels(query);
            return result;
        }

        [HttpGet]
        [Route("detail-hotels")]
        public async Task<HotelDetailViewModel> GetHotelDetail(Guid hotelId)
        {
            var result = await _hotelService.GetHotelDetail(hotelId);
            return result;
        }

        [Permission(CommonConstants.Permissions.ADD_HOTEL_PERMISSION)]
        [HttpPost]
        [Route("create-hotels")]
        public async Task<ResponseResult> CreateHotel([FromForm] HotelCreateUpdateViewModel model)
        {
            var result = await _hotelService.CreateHotel(model, CurrentUser);
            return result;
        }

        [Permission(CommonConstants.Permissions.UPDATE_HOTEL_PERMISSION)]
        [HttpPut]
        [Route("update-hotels")]
        public async Task<ResponseResult> UpdateHotel(Guid hotelId, [FromForm] HotelCreateUpdateViewModel model)
        {
            var result = await _hotelService.UpdateHotel(hotelId, model, CurrentUser);
            return result;
        }

        [Permission(CommonConstants.Permissions.DELETE_HOTEL_PERMISSION)]
        [HttpDelete]
        [Route("delete-hotels")]
        public async Task<ResponseResult> DeleteHotel(Guid hotelId)
        {
            var result = await _hotelService.DeleteHotel(hotelId);
            return result;
        }

        [Permission(CommonConstants.Permissions.UPDATE_HOTEL_PERMISSION)]
        [HttpPut]
        [Route("status-hotels")]
        public async Task<ResponseResult> UpdateStatus([FromBody] UpdateStatusViewModel model)
        {
            var result = await _hotelService.UpdateStatus(model);
            return result;
        }
   
}