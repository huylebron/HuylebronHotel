using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Images ;
using HotelProject . Domain . Model . RoomTypeViewModel ;
using HotelProject . Domain . Model . Users ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Logging ;
using Newtonsoft . Json ;

namespace HotelProject . Application . Services ;

public class RoomTypeService : IRoomTypeService
{
    private readonly IGenericRepository < RoomType , Guid > _roomTypeRepository ;
    private readonly IGenericRepository < Hotel , Guid > _hotelRepository ;
    private readonly IFileService _fileService ;
    private readonly IUnitOfWork _unitOfWork ;
    private readonly ILogger < RoomTypeService > _logger ;
    private const string _imageFolder = "roomTypeImages" ;

    public RoomTypeService (
        IGenericRepository < RoomType , Guid > roomTypeRepository ,
        IGenericRepository < Hotel , Guid > hotelRepository ,
        IUnitOfWork unitOfWork ,
        IFileService fileService ,
        ILogger < RoomTypeService > logger ) {
        _roomTypeRepository = roomTypeRepository ;
        _hotelRepository = hotelRepository ;
        _unitOfWork = unitOfWork ;
        _fileService = fileService ;
        _logger = logger ;
    }

#region GetRoomTypes

    public async Task < PageResult < RoomTypeViewModel > > GetRoomTypes ( SearchQuery query ) {
        var result = new PageResult < RoomTypeViewModel >
        {
            CurrentPage = query . PageIndex
        } ;

        var roomTypeQuery = _roomTypeRepository . FindAll ( includeProperties : rt => rt . Rooms ) ;

        // Lọc theo trạng thái nếu cần
        if ( query . DisplayActiveItem )
            roomTypeQuery = roomTypeQuery . Where ( s => s . Status == EntityStatus . Active ) ;

        // Lọc theo từ khóa nếu có
        if ( ! string . IsNullOrEmpty ( query . Keyword ) )
            roomTypeQuery = roomTypeQuery . Where ( s => s . Name . Contains ( query . Keyword ) ||
                                                         ( s . Description != null &&
                                                           s . Description . Contains ( query . Keyword ) ) ) ;

        // Đếm tổng số bản ghi thỏa mãn điều kiện query
        result . TotalCount = await roomTypeQuery . CountAsync ( ) ;

        // Lấy dữ liệu phân trang
        var roomTypes = await roomTypeQuery
                              . OrderBy ( s => s . Name )
                              . Skip ( query . SkipNo )
                              . Take ( query . TakeNo )
                              . Select ( s => new RoomTypeViewModel
                              {
                                  Id = s . Id ,
                                  Name = s . Name ,
                                  Description = s . Description ,
                                  TotalRooms = s . Rooms . Count
                              } )
                              . ToListAsync ( ) ;

        // Xử lý ảnh đầu tiên cho mỗi loại phòng
        foreach ( var roomType in roomTypes )
        {
            var roomTypeEntity = await _roomTypeRepository . FindByIdAsync ( roomType . Id ) ;
            if ( ! string . IsNullOrEmpty ( roomTypeEntity . ImageJson ) )
            {
                var images = JsonConvert . DeserializeObject < List < ImageInEntity > > ( roomTypeEntity . ImageJson ) ;
                roomType . ImageUrl = images . FirstOrDefault ( ) ? . ImageUrl ;
            }
        }

        result . Data = roomTypes ;
        return result ;
    }

#endregion

#region GetRoomTypeDetail

    public async Task < RoomTypeViewModel > GetRoomTypeDetail ( Guid roomTypeId ) {
        var roomType = await _roomTypeRepository . FindByIdAsync ( roomTypeId , rt => rt . Rooms , rt => rt . Hotel ) ;
        if ( roomType == null ) throw new RoomTypeException . RoomTypeNotFoundException ( roomTypeId ) ;

        var result = new RoomTypeViewModel
        {
            Id = roomType . Id ,
            Name = roomType . Name ,
            Description = roomType . Description ,
            TotalRooms = roomType . Rooms . Count
        } ;

        // Xử lý ảnh
        if ( ! string . IsNullOrEmpty ( roomType . ImageJson ) )
        {
            var images = JsonConvert . DeserializeObject < List < ImageInEntity > > ( roomType . ImageJson ) ;
            result . ImageUrl = images . FirstOrDefault ( ) ? . ImageUrl ;
        }

        return result ;
    }

#endregion

#region CreateRoomType

    public async Task < ResponseResult >
        CreateRoomType ( RoomTypeCreateUpdateViewModel model , UserProfileModel currentUser ) {
        var hotel = await _hotelRepository . FindByIdAsync ( model . HotelId ) ;
        if ( hotel == null ) throw new HotelException . HotelNotFoundException ( model . HotelId ) ;

        List < ImageInEntity > roomTypeImages = new List < ImageInEntity > ( ) ;

        // Xử lý upload ảnh
        if ( model . ImageFiles != null && model . ImageFiles . Any ( ) )
        {
            var uploadTasks = model . ImageFiles . Select ( img => _fileService . UploadFile ( img , _imageFolder ) ) ;
            var uploadedImages = await Task . WhenAll ( uploadTasks ) ;

            roomTypeImages = uploadedImages . Select ( img => new ImageInEntity (
                Guid . NewGuid ( ) ,
                img . FileName ,
                img . FilePath
            ) ) . ToList ( ) ;
        }

        var newRoomType = new RoomType
        {
            Id = Guid . NewGuid ( ) ,
            Name = model . Name ?? string . Empty ,
            Description = model . Description ,
            HotelId = model . HotelId ,
            ImageJson = roomTypeImages . Any ( ) ? JsonConvert . SerializeObject ( roomTypeImages ) : null ,
            CreatedBy = currentUser . UserId ,
            CreatedDate = DateTime . Now ,
            Status = EntityStatus . Active
        } ;

        try
        {
            _roomTypeRepository . Add ( newRoomType ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Tạo loại phòng mới thành công" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Lỗi khi tạo loại phòng mới" ) ;
            throw new RoomTypeException . UpdateRoomTypeException ( Guid . Empty ) ;
        }
    }

#endregion

#region UpdateRoomType

    public async Task < ResponseResult > UpdateRoomType ( Guid roomTypeId , RoomTypeCreateUpdateViewModel model ,
        UserProfileModel currentUser ) {
        var roomType = await _roomTypeRepository . FindByIdAsync ( roomTypeId ) ;
        if ( roomType == null ) throw new RoomTypeException . RoomTypeNotFoundException ( roomTypeId ) ;

        // Kiểm tra khách sạn tồn tại
        var hotel = await _hotelRepository . FindByIdAsync ( model . HotelId ) ;
        if ( hotel == null ) throw new HotelException . HotelNotFoundException ( model . HotelId ) ;

        roomType . Name = model . Name ?? roomType . Name ;
        roomType . Description = model . Description ;
        roomType . HotelId = model . HotelId ;
        roomType . UpdatedBy = currentUser . UserId ;
        roomType . UpdatedDate = DateTime . Now ;

        // Xử lý update ảnh 
        if ( model . ImageFiles != null && model . ImageFiles . Any ( ) )
        {
            List < ImageInEntity > roomTypeImages = new List < ImageInEntity > ( ) ;

            // Giữ lại ảnh cũ nếu có
            if ( ! string . IsNullOrEmpty ( roomType . ImageJson ) )
                roomTypeImages = JsonConvert . DeserializeObject < List < ImageInEntity > > ( roomType . ImageJson ) ;

            // Thêm ảnh mới
            var uploadTasks = model . ImageFiles . Select ( img => _fileService . UploadFile ( img , _imageFolder ) ) ;
            var uploadedImages = await Task . WhenAll ( uploadTasks ) ;

            var newImages = uploadedImages . Select ( img => new ImageInEntity (
                Guid . NewGuid ( ) ,
                img . FileName ,
                img . FilePath
            ) ) . ToList ( ) ;

            roomTypeImages . AddRange ( newImages ) ;
            roomType . ImageJson = JsonConvert . SerializeObject ( roomTypeImages ) ;
        }

        try
        {
            _roomTypeRepository . Update ( roomType ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Cập nhật loại phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Lỗi khi cập nhật loại phòng với ID: {RoomTypeId}" , roomTypeId ) ;
            throw new RoomTypeException . UpdateRoomTypeException ( roomTypeId ) ;
        }
    }

#endregion

#region DeleteRoomType

    public async Task < ResponseResult > DeleteRoomType ( Guid roomTypeId ) {
        var roomType = await _roomTypeRepository . FindByIdAsync ( roomTypeId ) ;
        if ( roomType == null ) throw new RoomTypeException . RoomTypeNotFoundException ( roomTypeId ) ;

        try
        {
            _roomTypeRepository . Remove ( roomType ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Xóa loại phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Lỗi khi xóa loại phòng với ID: {RoomTypeId}" , roomTypeId ) ;
            throw new RoomTypeException . DeleteRoomTypeException ( roomTypeId ) ;
        }
    }

#endregion

#region UpdateStatus

    public async Task < ResponseResult > UpdateStatus ( UpdateStatusViewModel model ) {
        var roomType = await _roomTypeRepository . FindByIdAsync ( model . Id ) ;
        if ( roomType == null ) throw new RoomTypeException . RoomTypeNotFoundException ( model . Id ) ;

        roomType . Status = model . Status ;

        try
        {
            _roomTypeRepository . Update ( roomType ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Cập nhật trạng thái loại phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Lỗi khi cập nhật trạng thái loại phòng có ID: {RoomTypeId}" , model . Id ) ;
            throw new RoomTypeException . UpdateRoomTypeException ( model . Id ) ;
        }
    }

#endregion
}