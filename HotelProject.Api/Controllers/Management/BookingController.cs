using HotelProject . Api . Controllers . Bases ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Booking ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Payment ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject.Api.Controllers.Management ;

public class BookingController : AuthorizeController
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [Route("get-bookings")]
    public async Task<PageResult<BookingListViewModel>> GetBookings([FromBody] SearchQuery query)
    {
        var result = await _bookingService.GetBookings(query);
        return result;
    }

    [HttpGet]
    [Route("detail-booking")]
    public async Task<BookingDetailViewModel> GetBookingDetail(Guid bookingId)
    {
        var result = await _bookingService.GetBookingDetail(bookingId);
        return result;
    }

    [HttpPost]
    [Route("create-booking")]
    public async Task<ResponseResult> CreateBooking([FromBody] BookingCreateViewModel model)
    {
        var result = await _bookingService.CreateBooking(model, CurrentUser);
        return result;
    }

    [HttpPut]
    [Route("update-booking-status")]
    public async Task<ResponseResult> UpdateBookingStatus([FromBody] BookingUpdateStatusViewModel model)
    {
        var result = await _bookingService.UpdateBookingStatus(model, CurrentUser);
        return result;
    }

    [HttpDelete]
    [Route("cancel-booking")]
    public async Task<ResponseResult> CancelBooking(Guid bookingId)
    {
        var result = await _bookingService.CancelBooking(bookingId, CurrentUser);
        return result;
    }

    [HttpPost]
    [Route("process-payment")]
    public async Task<ResponseResult> ProcessPayment([FromBody] PaymentRequestViewModel model)
    {
        var result = await _bookingService.ProcessPayment(model, CurrentUser);
        return result;
    }
  
}