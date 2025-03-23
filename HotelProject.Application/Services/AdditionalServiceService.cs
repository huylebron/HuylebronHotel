using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . AdditionalService ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Images ;
using HotelProject . Domain . Model . Users ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Logging ;
using Newtonsoft . Json ;

namespace HotelProject . Application . Services ;

public class AdditionalServiceService : IAdditionalServiceService
{
#region CreateService

    public async Task < ResponseResult > CreateService ( AdditionalServiceCreateUpdateViewModel model ,
        UserProfileModel currentUser ) {
        var serviceImages = new List < ImageInEntity > ( ) ;

        if ( model . ImageFile != null )
        {
            var uploadedImage = await _fileService . UploadFile ( model . ImageFile , _imageFolder ) ;
            serviceImages . Add ( new ImageInEntity (
                Guid . NewGuid ( ) ,
                uploadedImage . FileName ,
                uploadedImage . FilePath
            ) ) ;
        }

        var newService = new AdditionalService
        {
            Id = Guid . NewGuid ( ) ,
            Name = model . Name ,
            Description = model . Description ,
            Price = model . Price ,
            ImageJson = serviceImages . Any ( ) ? JsonConvert . SerializeObject ( serviceImages ) : null ,
            CreatedBy = currentUser . UserId ,
            CreatedDate = DateTime . Now ,
            Status = EntityStatus . Active
        } ;

        try
        {
            _serviceRepository . Add ( newService ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Service created successfully" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Error creating new service" ) ;
            throw new AdditionalServiceException . UpdateServiceException ( Guid . Empty ) ;
        }
    }

#endregion

#region UpdateService

    public async Task < ResponseResult > UpdateService ( Guid serviceId , AdditionalServiceCreateUpdateViewModel model ,
        UserProfileModel currentUser ) {
        var service = await _serviceRepository . FindByIdAsync ( serviceId ) ;
        if ( service == null ) throw new AdditionalServiceException . ServiceNotFoundException ( serviceId ) ;

        service . Name = model . Name ;
        service . Description = model . Description ;
        service . Price = model . Price ;
        service . UpdatedBy = currentUser . UserId ;
        service . UpdatedDate = DateTime . Now ;

        if ( model . ImageFile != null )
        {
            var serviceImages = new List < ImageInEntity > ( ) ;

            var uploadedImage = await _fileService . UploadFile ( model . ImageFile , _imageFolder ) ;
            serviceImages . Add ( new ImageInEntity (
                Guid . NewGuid ( ) ,
                uploadedImage . FileName ,
                uploadedImage . FilePath
            ) ) ;

            service . ImageJson = JsonConvert . SerializeObject ( serviceImages ) ;
        }

        try
        {
            _serviceRepository . Update ( service ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Service updated successfully" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Error updating service with ID: {ServiceId}" , serviceId ) ;
            throw new AdditionalServiceException . UpdateServiceException ( serviceId ) ;
        }
    }

#endregion


#region DeleteService

    public async Task < ResponseResult > DeleteService ( Guid serviceId ) {
        var service = await _serviceRepository . FindByIdAsync ( serviceId ) ;
        if ( service == null ) throw new AdditionalServiceException . ServiceNotFoundException ( serviceId ) ;

        var usedInBookings =
            await _bookingServiceRepository . FindAll ( bs => bs . ServiceId == serviceId ) . AnyAsync ( ) ;
        if ( usedInBookings ) return ResponseResult . Fail ( "Cannot delete service in use" ) ;

        try
        {
            _serviceRepository . Remove ( service ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Service deleted successfully" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Error deleting service with ID: {ServiceId}" , serviceId ) ;
            throw new AdditionalServiceException . DeleteServiceException ( serviceId ) ;
        }
    }

#endregion


    public async Task < ResponseResult > UpdateStatus ( UpdateStatusViewModel model ) {
        var service = await _serviceRepository . FindByIdAsync ( model . Id ) ;
        if ( service == null ) throw new AdditionalServiceException . ServiceNotFoundException ( model . Id ) ;

        service . Status = model . Status ;

        try
        {
            _serviceRepository . Update ( service ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Service status updated successfully" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Error updating status for service ID: {ServiceId}" , model . Id ) ;
            throw new AdditionalServiceException . UpdateServiceException ( model . Id ) ;
        }
    }

#region Constructor

    public AdditionalServiceService (
        IGenericRepository < AdditionalService , Guid > serviceRepository ,
        IGenericRepository < BookingService , Guid > bookingServiceRepository ,
        IUnitOfWork unitOfWork ,
        IFileService fileService ,
        ILogger < AdditionalServiceService > logger ) {
        _serviceRepository = serviceRepository ;
        _bookingServiceRepository = bookingServiceRepository ;
        _unitOfWork = unitOfWork ;
        _fileService = fileService ;
        _logger = logger ;
    }

#endregion

#region Fields

    private readonly IGenericRepository < AdditionalService , Guid > _serviceRepository ;
    private readonly IGenericRepository < BookingService , Guid > _bookingServiceRepository ;
    private readonly IFileService _fileService ;
    private readonly IUnitOfWork _unitOfWork ;
    private readonly ILogger < AdditionalServiceService > _logger ;
    private const string _imageFolder = "serviceImages" ;

#endregion

#region Query Methods

    public async Task < PageResult < AdditionalServiceViewModel > > GetServices ( SearchQuery query ) {
        var result = new PageResult < AdditionalServiceViewModel >
        {
            CurrentPage = query . PageIndex
        } ;

        var serviceQuery = _serviceRepository . FindAll ( ) ;

        if ( query . DisplayActiveItem )
            serviceQuery = serviceQuery . Where ( s => s . Status == EntityStatus . Active ) ;

        if ( ! string . IsNullOrEmpty ( query . Keyword ) )
            serviceQuery = serviceQuery . Where ( s => s . Name . Contains ( query . Keyword ) ||
                                                       ( s . Description != null &&
                                                         s . Description . Contains ( query . Keyword ) ) ) ;

        result . TotalCount = await serviceQuery . CountAsync ( ) ;

        var services = await serviceQuery
                             . OrderBy ( s => s . Name )
                             . Skip ( query . SkipNo )
                             . Take ( query . TakeNo )
                             . Select ( s => new AdditionalServiceViewModel
                             {
                                 Id = s . Id ,
                                 Name = s . Name ,
                                 Description = s . Description ,
                                 Price = s . Price
                             } )
                             . ToListAsync ( ) ;

        foreach ( var service in services )
        {
            var serviceEntity = await _serviceRepository . FindByIdAsync ( service . Id ) ;
            if ( ! string . IsNullOrEmpty ( serviceEntity . ImageJson ) )
            {
                var images = JsonConvert . DeserializeObject < List < ImageInEntity > > ( serviceEntity . ImageJson ) ;
                service . ImageUrl = images . FirstOrDefault ( ) ? . ImageUrl ;
            }
        }

        result . Data = services ;
        return result ;
    }

    public async Task < AdditionalServiceViewModel > GetServiceDetail ( Guid serviceId ) {
        var service = await _serviceRepository . FindByIdAsync ( serviceId ) ;
        if ( service == null ) throw new AdditionalServiceException . ServiceNotFoundException ( serviceId ) ;

        var result = new AdditionalServiceViewModel
        {
            Id = service . Id ,
            Name = service . Name ,
            Description = service . Description ,
            Price = service . Price
        } ;

        if ( ! string . IsNullOrEmpty ( service . ImageJson ) )
        {
            var images = JsonConvert . DeserializeObject < List < ImageInEntity > > ( service . ImageJson ) ;
            result . ImageUrl = images . FirstOrDefault ( ) ? . ImageUrl ;
        }

        return result ;
    }

#endregion
}