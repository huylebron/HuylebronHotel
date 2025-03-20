using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Hotel ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IHotelService
{
    Task<PageResult<HotelListViewModel>> GetHotels(SearchQuery query);
    Task<HotelDetailViewModel> GetHotelDetail(Guid hotelId);
    Task<ResponseResult> CreateHotel(HotelCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> UpdateHotel(Guid hotelId, HotelCreateUpdateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> DeleteHotel(Guid hotelId);
    Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model);
}