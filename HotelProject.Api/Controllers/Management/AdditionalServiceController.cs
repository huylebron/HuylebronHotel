using HotelProject . Api . Controllers . Bases ;
using HotelProject . Api . Filters ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . AdditionalService ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Utility ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject . Api . Controllers . Management ;

[ Route ( "api/[controller]" ) ]
[ ApiController ]
public class AdditionalServiceController : AuthorizeController
{
    private readonly IAdditionalServiceService _serviceService ;

    public AdditionalServiceController ( IAdditionalServiceService serviceService ) {
        _serviceService = serviceService ;
    }

#region get

    [ HttpPost ]
    [ Route ( "get-services" ) ]
    public async Task < PageResult < AdditionalServiceViewModel > > GetServices ( [ FromBody ] SearchQuery query ) {
        var result = await _serviceService . GetServices ( query ) ;
        return result ;
    }

#endregion


#region detail

    [ HttpGet ]
    [ Route ( "detail-service" ) ]
    public async Task < AdditionalServiceViewModel > GetServiceDetail ( Guid serviceId ) {
        var result = await _serviceService . GetServiceDetail ( serviceId ) ;
        return result ;
    }

#endregion


#region create

    [ Permission ( CommonConstants . Permissions . ADD_SERVICE_PERMISSION ) ]
    [ HttpPost ]
    [ Route ( "create-service" ) ]
    public async Task < ResponseResult > CreateService ( [ FromForm ] AdditionalServiceCreateUpdateViewModel model ) {
        var result = await _serviceService . CreateService ( model , CurrentUser ) ;
        return result ;
    }

#endregion


#region update

    [ Permission ( CommonConstants . Permissions . UPDATE_SERVICE_PERMISSION ) ]
    [ HttpPut ]
    [ Route ( "update-service" ) ]
    public async Task < ResponseResult > UpdateService ( Guid serviceId ,
        [ FromForm ] AdditionalServiceCreateUpdateViewModel model ) {
        var result = await _serviceService . UpdateService ( serviceId , model , CurrentUser ) ;
        return result ;
    }

#endregion


#region delete

    [ Permission ( CommonConstants . Permissions . DELETE_SERVICE_PERMISSION ) ]
    [ HttpDelete ]
    [ Route ( "delete-service" ) ]
    public async Task < ResponseResult > DeleteService ( Guid serviceId ) {
        var result = await _serviceService . DeleteService ( serviceId ) ;
        return result ;
    }

    [ Permission ( CommonConstants . Permissions . UPDATE_SERVICE_PERMISSION ) ]
    [ HttpPut ]
    [ Route ( "status-service" ) ]
    public async Task < ResponseResult > UpdateStatus ( [ FromBody ] UpdateStatusViewModel model ) {
        var result = await _serviceService . UpdateStatus ( model ) ;
        return result ;
    }

#endregion
}