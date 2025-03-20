using HotelProject . Domain . Model . Booking ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IBookingService
{
    Task<PageResult<BookingListViewModel>> GetBookings(SearchQuery query);
    Task<BookingDetailViewModel> GetBookingDetail(Guid bookingId);
    Task<ResponseResult> CreateBooking(BookingCreateViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> UpdateBookingStatus(BookingUpdateStatusViewModel model, UserProfileModel currentUser);
    Task<ResponseResult> CancelBooking(Guid bookingId, UserProfileModel currentUser);
}