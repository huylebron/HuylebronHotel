using HotelProject . Api . Controllers . Bases ;
using HotelProject . Api . Filters ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Amenity ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Utility ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject . Api . Controllers . Management ;

[ Route ( "api/[controller]" ) ]
[ ApiController ]
[ ApplicationAuthorize ]
public class AmenityController : AuthorizeController
{
    private readonly IAmenityService _amenityService ;

    public AmenityController ( IAmenityService amenityService ) {
        _amenityService = amenityService ;
    }

#region get

    [ HttpPost ]
    [ Route ( "get-amenities" ) ]
    public async Task < PageResult < AmenityViewModel > > GetAmenities ( [ FromBody ] SearchQuery query ) {
        var result = await _amenityService . GetAmenities ( query ) ;
        return result ;
    }

#endregion


#region detail

    [ HttpGet ]
    [ Route ( "detail-amenity" ) ]
    public async Task < AmenityViewModel > GetAmenityDetail ( Guid amenityId ) {
        var result = await _amenityService . GetAmenityDetail ( amenityId ) ;
        return result ;
    }

#endregion


#region create

    [ Permission ( CommonConstants . Permissions . ADD_AMENITY_PERMISSION ) ]
    [ HttpPost ]
    [ Route ( "create-amenity" ) ]
    [ ApplicationAuthorize ]
    public async Task < ResponseResult > CreateAmenity ( [ FromBody ] AmenityCreateUpdateViewModel model ) {
        var result = await _amenityService . CreateAmenity ( model , CurrentUser ) ;
        return result ;
    }

#endregion


#region update

    [ Permission ( CommonConstants . Permissions . UPDATE_AMENITY_PERMISSION ) ]
    [ HttpPut ]
    [ Route ( "update-amenity" ) ]
    public async Task < ResponseResult > UpdateAmenity ( Guid amenityId ,
        [ FromBody ] AmenityCreateUpdateViewModel model ) {
        var result = await _amenityService . UpdateAmenity ( amenityId , model , CurrentUser ) ;
        return result ;
    }

#endregion


#region delete

    [ Permission ( CommonConstants . Permissions . DELETE_AMENITY_PERMISSION ) ]
    [ HttpDelete ]
    [ Route ( "delete-amenity" ) ]
    public async Task < ResponseResult > DeleteAmenity ( Guid amenityId ) {
        var result = await _amenityService . DeleteAmenity ( amenityId ) ;
        return result ;
    }

    [ Permission ( CommonConstants . Permissions . UPDATE_AMENITY_PERMISSION ) ]
    [ HttpPut ]
    [ Route ( "status-amenity" ) ]
    public async Task < ResponseResult > UpdateStatus ( [ FromBody ] UpdateStatusViewModel model ) {
        var result = await _amenityService . UpdateStatus ( model ) ;
        return result ;
    }

#endregion
}