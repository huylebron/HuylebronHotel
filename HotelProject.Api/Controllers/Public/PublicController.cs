using HotelProject . Api . Controllers . Bases ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Hotel ;
using HotelProject . Domain . Model . Room ;
using HotelProject . Domain . Model . RoomTypeViewModel ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject . Api . Controllers . Public ;

[ Route ( "api/[controller]" ) ]
[ ApiController ]
public class PublicController : NoAuthorizeController
{
    private readonly IHotelService _hotelService ;
    private readonly IRoomService _roomService ;
    private readonly IRoomTypeService _roomTypeService ;

    public PublicController (
        IHotelService hotelService ,
        IRoomTypeService roomTypeService ,
        IRoomService roomService ) {
        _hotelService = hotelService ;
        _roomTypeService = roomTypeService ;
        _roomService = roomService ;
    }

    [ HttpPost ]
    [ Route ( "hotels" ) ]
    public async Task < PageResult < HotelListViewModel > > GetHotels ( [ FromBody ] SearchQuery query ) {
        // Chỉ hiển thị các khách sạn đang hoạt động
        query . DisplayActiveItem = true ;
        var result = await _hotelService . GetHotels ( query ) ;
        return result ;
    }

    [ HttpGet ]
    [ Route ( "hotel-detail" ) ]
    public async Task < HotelDetailViewModel > GetHotelDetail ( Guid hotelId ) {
        var result = await _hotelService . GetHotelDetail ( hotelId ) ;
        return result ;
    }

    [ HttpPost ]
    [ Route ( "room-types" ) ]
    public async Task < PageResult < RoomTypeViewModel > > GetRoomTypes ( [ FromBody ] SearchQuery query ) {
        // Chỉ hiển thị các loại phòng đang hoạt động
        query . DisplayActiveItem = true ;
        var result = await _roomTypeService . GetRoomTypes ( query ) ;
        return result ;
    }

    [ HttpGet ]
    [ Route ( "room-type-detail" ) ]
    public async Task < RoomTypeViewModel > GetRoomTypeDetail ( Guid roomTypeId ) {
        var result = await _roomTypeService . GetRoomTypeDetail ( roomTypeId ) ;
        return result ;
    }

    [ HttpGet ]
    [ Route ( "room-detail" ) ]
    public async Task < RoomDetailViewModel > GetRoomDetail ( Guid roomId ) {
        var result = await _roomService . GetRoomDetail ( roomId ) ;
        return result ;
    }

    [ HttpPost ]
    [ Route ( "search-rooms" ) ]
    public async Task < List < RoomListViewModel > > SearchAvailableRooms ( [ FromBody ] RoomSearchViewModel model ) {
        var result = await _roomService . SearchAvailableRooms ( model ) ;
        return result ;
    }
}