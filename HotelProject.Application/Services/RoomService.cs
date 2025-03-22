using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . Amenity ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Images ;
using HotelProject . Domain . Model . Review ;
using HotelProject . Domain . Model . Room ;
using HotelProject . Domain . Model . Users ;
using Microsoft . AspNetCore . Http ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Logging ;
using Newtonsoft . Json ;

namespace HotelProject . Application . Services ;

public class RoomService : IRoomService
{
    private readonly IGenericRepository < Room , Guid > _roomRepository ;
    private readonly IGenericRepository < RoomType , Guid > _roomTypeRepository ;
    private readonly IGenericRepository < Amenity , Guid > _amenityRepository ;
    private readonly IGenericRepository < RoomAmenity , Guid > _roomAmenityRepository ;
    private readonly IGenericRepository < Booking , Guid > _bookingRepository ;
    private readonly IFileService _fileService ;
    private readonly IUnitOfWork _unitOfWork ;
    private readonly ILogger < RoomService > _logger ;
    private const string _imageFolder = "roomImages" ;

    public RoomService (
        IGenericRepository < Room , Guid > roomRepository ,
        IGenericRepository < RoomType , Guid > roomTypeRepository ,
        IGenericRepository < Amenity , Guid > amenityRepository ,
        IGenericRepository < RoomAmenity , Guid > roomAmenityRepository ,
        IGenericRepository < Booking , Guid > bookingRepository ,
        IUnitOfWork unitOfWork ,
        IFileService fileService ,
        ILogger < RoomService > logger ) {
        _roomRepository = roomRepository ;
        _roomTypeRepository = roomTypeRepository ;
        _amenityRepository = amenityRepository ;
        _roomAmenityRepository = roomAmenityRepository ;
        _bookingRepository = bookingRepository ;
        _unitOfWork = unitOfWork ;
        _fileService = fileService ;
        _logger = logger ;
    }

    public async Task < PageResult < RoomListViewModel > > GetRooms ( SearchQuery query ) {
        var result = new PageResult < RoomListViewModel >
        {
            CurrentPage = query . PageIndex
        } ;

        var roomQuery = _roomRepository . FindAll ( includeProperties : r => r . RoomType ) ;

        // Lọc theo trạng thái nếu cần
        if ( query . DisplayActiveItem )
        {
            roomQuery = roomQuery . Where ( s => s . Status == EntityStatus . Active ) ;
        }

        // Lọc theo từ khóa nếu có
        if ( ! string . IsNullOrEmpty ( query . Keyword ) )
        {
            roomQuery = roomQuery . Where ( s => s . Name . Contains ( query . Keyword ) ||
                                                 s . RoomNumber . Contains ( query . Keyword ) ||
                                                 ( s . Description != null &&
                                                   s . Description . Contains ( query . Keyword ) ) ) ;
        }

        // Đếm tổng số bản ghi thỏa mãn điều kiện query
        result . TotalCount = await roomQuery . CountAsync ( ) ;

        // Lấy dữ liệu phân trang
        var rooms = await roomQuery
                          . OrderBy ( s => s . RoomNumber )
                          . Skip ( query . SkipNo )
                          . Take ( query . TakeNo )
                          . Select ( s => new RoomListViewModel
                          {
                              Id = s . Id ,
                              RoomNumber = s . RoomNumber ,
                              Name = s . Name ,
                              MaxOccupancy = s . MaxOccupancy ,
                              AreaInSquareMeters = s . AreaInSquareMeters ,
                              PricePerNight = s . PricePerNight ,
                              DiscountPrice = s . DiscountPrice ,
                              RoomTypeName = s . RoomType . Name ,
                              RoomStatus = s . RoomStatus ,
                              RoomStatusName = s . RoomStatus . ToString ( )
                          } )
                          . ToListAsync ( ) ;

        // Xử lý ảnh đầu tiên cho mỗi phòng
        foreach ( var room in rooms )
        {
            var roomEntity = await _roomRepository . FindByIdAsync ( room . Id ) ;
            if ( ! string . IsNullOrEmpty ( roomEntity . ImageJson ) )
            {
                var images = JsonConvert . DeserializeObject < List < ImageInEntity > > ( roomEntity . ImageJson ) ;
                room . ImageUrl = images . FirstOrDefault ( ) ? . ImageUrl ;
            }
        }

        result . Data = rooms ;
        return result ;
    }

    public async Task < RoomDetailViewModel > GetRoomDetail ( Guid roomId ) {
        var room = await _roomRepository . FindByIdAsync ( roomId ,
            r => r . RoomType ,
            r => r . Reviews ,
            r => r . RoomAmenities ) ;

        if ( room == null )
        {
            throw new RoomException . RoomNotFoundException ( roomId ) ;
        }

        // Get hotel name
        var hotel = await _roomTypeRepository . FindByIdAsync ( room . RoomTypeId , rt => rt . Hotel ) ;
        string hotelName = hotel ? . Hotel ? . Name ?? "Unknown Hotel" ;

        // Get amenities
        var roomAmenityIds = room . RoomAmenities . Select ( ra => ra . AmenityId ) . ToList ( ) ;
        var amenities = await _amenityRepository . FindAll ( a => roomAmenityIds . Contains ( a . Id ) )
                                                 . Select ( a => new AmenityViewModel
                                                 {
                                                     Id = a . Id ,
                                                     Name = a . Name ,
                                                     Description = a . Description
                                                 } )
                                                 . ToListAsync ( ) ;

        // Get reviews
        var reviews = room . Reviews . Select ( r => new RoomReviewViewModel
        {
            Id = r . Id ,
            ReviewerName = r . ReviewerName ,
            Content = r . Content ,
            Rating = r . Rating ,
            CreatedDate = r . CreatedDate ?? DateTime . MinValue
        } ) . ToList ( ) ;

        var result = new RoomDetailViewModel
        {
            Id = room . Id ,
            RoomNumber = room . RoomNumber ,
            Name = room . Name ,
            Description = room . Description ,
            MaxOccupancy = room . MaxOccupancy ,
            AreaInSquareMeters = room . AreaInSquareMeters ,
            PricePerNight = room . PricePerNight ,
            DiscountPrice = room . DiscountPrice ,
            RoomTypeName = room . RoomType . Name ,
            RoomTypeId = room . RoomTypeId ,
            HotelName = hotelName ,
            RoomStatus = room . RoomStatus ,
            RoomStatusName = room . RoomStatus . ToString ( ) ,
            Amenities = amenities ,
            Reviews = reviews
        } ;

        // Xử lý ảnh
        if ( ! string . IsNullOrEmpty ( room . ImageJson ) )
        {
            var images = JsonConvert . DeserializeObject < List < ImageInEntity > > ( room . ImageJson ) ;
            result . Images = images . Select ( img => img . ImageUrl ) . ToList ( ) ;
        }

        return result ;
    }

    public async Task < ResponseResult > CreateRoom ( RoomCreateUpdateViewModel model , UserProfileModel currentUser ) {
        // Kiểm tra loại phòng tồn tại
        var roomType = await _roomTypeRepository . FindByIdAsync ( model . RoomTypeId ) ;
        if ( roomType == null )
        {
            throw new RoomTypeException . RoomTypeNotFoundException ( model . RoomTypeId ) ;
        }

        // Kiểm tra mã phòng đã tồn tại chưa
        var existingRoom = await _roomRepository . FindSingleAsync ( r => r . RoomNumber == model . RoomNumber ) ;
        if ( existingRoom != null )
        {
            return ResponseResult . Fail ( "Room number already exists" ) ;
        }

        List < ImageInEntity > roomImages = new List < ImageInEntity > ( ) ;

        // Xử lý upload ảnh
        if ( model . ImageFiles != null && model . ImageFiles . Any ( ) )
        {
            var uploadTasks = model . ImageFiles . Select ( img => _fileService . UploadFile ( img , _imageFolder ) ) ;
            var uploadedImages = await Task . WhenAll ( uploadTasks ) ;

            roomImages = uploadedImages . Select ( img => new ImageInEntity (
                Guid . NewGuid ( ) ,
                img . FileName ,
                img . FilePath
            ) ) . ToList ( ) ;
        }

        await _unitOfWork . BeginTransactionAsync ( ) ;

        try
        {
            // Tạo phòng mới
            var newRoom = new Room
            {
                Id = Guid . NewGuid ( ) ,
                RoomNumber = model . RoomNumber ,
                Name = model . Name ,
                Description = model . Description ,
                MaxOccupancy = model . MaxOccupancy ,
                AreaInSquareMeters = model . AreaInSquareMeters ,
                PricePerNight = model . PricePerNight ,
                DiscountPrice = model . DiscountPrice ,
                RoomTypeId = model . RoomTypeId ,
                RoomStatus = model . RoomStatus ,
                ImageJson = roomImages . Any ( ) ? JsonConvert . SerializeObject ( roomImages ) : null ,
                CreatedBy = currentUser . UserId ,
                CreatedDate = DateTime . Now ,
                Status = EntityStatus . Active
            } ;

            _roomRepository . Add ( newRoom ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;

            // Thêm các tiện nghi nếu có
            if ( model . AmenityIds != null && model . AmenityIds . Any ( ) )
            {
                var roomAmenities = model . AmenityIds . Select ( amenityId => new RoomAmenity
                {
                    Id = Guid . NewGuid ( ) ,
                    RoomId = newRoom . Id ,
                    AmenityId = amenityId
                } ) . ToList ( ) ;

                _roomAmenityRepository . AddRange ( roomAmenities ) ;
                await _unitOfWork . SaveChangesAsync ( ) ;
            }

            await _unitOfWork . CommitAsync ( ) ;
            return ResponseResult . Success ( "Tạo phòng mới thành công" ) ;
        }
        catch ( Exception ex )
        {
            await _unitOfWork . RollbackAsync ( ) ;
            _logger . LogError ( ex , "Lỗi khi tạo phòng mới" ) ;
            throw new RoomException . UpdateRoomException ( Guid . Empty ) ;
        }
    }

    public async Task < ResponseResult > UpdateRoom ( Guid roomId , RoomCreateUpdateViewModel model ,
        UserProfileModel currentUser ) {
        var room = await _roomRepository . FindByIdAsync ( roomId ) ;
        if ( room == null )
        {
            throw new RoomException . RoomNotFoundException ( roomId ) ;
        }

        // Kiểm tra loại phòng tồn tại
        var roomType = await _roomTypeRepository . FindByIdAsync ( model . RoomTypeId ) ;
        if ( roomType == null )
        {
            throw new RoomTypeException . RoomTypeNotFoundException ( model . RoomTypeId ) ;
        }

        // Kiểm tra mã phòng đã tồn tại chưa (nếu có thay đổi)
        if ( room . RoomNumber != model . RoomNumber )
        {
            var existingRoom = await _roomRepository . FindSingleAsync ( r => r . RoomNumber == model . RoomNumber ) ;
            if ( existingRoom != null )
            {
                return ResponseResult . Fail ( "Room number already exists" ) ;
            }
        }

        // Xử lý update ảnh 
        if ( model . ImageFiles != null && model . ImageFiles . Any ( ) )
        {
            List < ImageInEntity > roomImages = new List < ImageInEntity > ( ) ;

            // Giữ lại ảnh cũ nếu có
            if ( ! string . IsNullOrEmpty ( room . ImageJson ) )
            {
                roomImages = JsonConvert . DeserializeObject < List < ImageInEntity > > ( room . ImageJson ) ;
            }

            // Thêm ảnh mới
            var uploadTasks = model . ImageFiles . Select ( img => _fileService . UploadFile ( img , _imageFolder ) ) ;
            var uploadedImages = await Task . WhenAll ( uploadTasks ) ;

            var newImages = uploadedImages . Select ( img => new ImageInEntity (
                Guid . NewGuid ( ) ,
                img . FileName ,
                img . FilePath
            ) ) . ToList ( ) ;

            roomImages . AddRange ( newImages ) ;
            room . ImageJson = JsonConvert . SerializeObject ( roomImages ) ;
        }

        await _unitOfWork . BeginTransactionAsync ( ) ;

        try
        {
            // Cập nhật thông tin phòng
            room . RoomNumber = model . RoomNumber ;
            room . Name = model . Name ;
            room . Description = model . Description ;
            room . MaxOccupancy = model . MaxOccupancy ;
            room . AreaInSquareMeters = model . AreaInSquareMeters ;
            room . PricePerNight = model . PricePerNight ;
            room . DiscountPrice = model . DiscountPrice ;
            room . RoomTypeId = model . RoomTypeId ;
            room . RoomStatus = model . RoomStatus ;
            room . UpdatedBy = currentUser . UserId ;
            room . UpdatedDate = DateTime . Now ;

            _roomRepository . Update ( room ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;

            // Cập nhật tiện nghi
            if ( model . AmenityIds != null )
            {
                // Xóa các tiện nghi cũ
                var currentAmenities =
                    await _roomAmenityRepository . FindAll ( ra => ra . RoomId == roomId ) . ToListAsync ( ) ;
                _roomAmenityRepository . RemoveMultiple ( currentAmenities ) ;
                await _unitOfWork . SaveChangesAsync ( ) ;

                // Thêm các tiện nghi mới
                if ( model . AmenityIds . Any ( ) )
                {
                    var roomAmenities = model . AmenityIds . Select ( amenityId => new RoomAmenity
                    {
                        Id = Guid . NewGuid ( ) ,
                        RoomId = room . Id ,
                        AmenityId = amenityId
                    } ) . ToList ( ) ;

                    _roomAmenityRepository . AddRange ( roomAmenities ) ;
                    await _unitOfWork . SaveChangesAsync ( ) ;
                }
            }

            await _unitOfWork . CommitAsync ( ) ;
            return ResponseResult . Success ( "Cập nhật phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            await _unitOfWork . RollbackAsync ( ) ;
            _logger . LogError ( ex , "Lỗi khi cập nhật phòng với ID: {RoomId}" , roomId ) ;
            throw new RoomException . UpdateRoomException ( roomId ) ;
        }
    }

    public async Task < ResponseResult > DeleteRoom ( Guid roomId ) {
        var room = await _roomRepository . FindByIdAsync ( roomId ) ;
        if ( room == null )
        {
            throw new RoomException . RoomNotFoundException ( roomId ) ;
        }

        // Kiểm tra xem phòng đã có booking chưa
        var bookings = await _bookingRepository . FindAll ( b => b . RoomId == roomId ) . ToListAsync ( ) ;
        if ( bookings . Any ( ) )
        {
            return ResponseResult . Fail ( "Không thể xóa phòng đã có booking" ) ;
        }

        await _unitOfWork . BeginTransactionAsync ( ) ;

        try
        {
            // Xóa các tiện nghi của phòng
            var roomAmenities =
                await _roomAmenityRepository . FindAll ( ra => ra . RoomId == roomId ) . ToListAsync ( ) ;
            _roomAmenityRepository . RemoveMultiple ( roomAmenities ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;

            // Xóa phòng
            _roomRepository . Remove ( room ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;

            await _unitOfWork . CommitAsync ( ) ;
            return ResponseResult . Success ( "Xóa phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            await _unitOfWork . RollbackAsync ( ) ;
            _logger . LogError ( ex , "Lỗi khi xóa phòng với ID: {RoomId}" , roomId ) ;
            throw new RoomException . DeleteRoomException ( roomId ) ;
        }
    }

    public async Task < ResponseResult > UpdateStatus ( UpdateStatusViewModel model ) {
        var room = await _roomRepository . FindByIdAsync ( model . Id ) ;
        if ( room == null )
        {
            throw new RoomException . RoomNotFoundException ( model . Id ) ;
        }

        room . Status = model . Status ;

        try
        {
            _roomRepository . Update ( room ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            return ResponseResult . Success ( "Cập nhật trạng thái phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            _logger . LogError ( ex , "Lỗi khi cập nhật trạng thái phòng có ID: {RoomId}" , model . Id ) ;
            throw new RoomException . UpdateRoomException ( model . Id ) ;
        }
    }

    public async Task < List < RoomListViewModel > > SearchAvailableRooms ( RoomSearchViewModel searchModel ) {
        var query = _roomRepository . FindAll (
            r => r . Status == EntityStatus . Active && r . RoomStatus == RoomStatus . Available ,
            r => r . RoomType ,
            r => r . RoomAmenities ) ;

        // Lọc theo khách sạn
        if ( searchModel . HotelId . HasValue )
        {
            query = query . Where ( r => r . RoomType . HotelId == searchModel . HotelId . Value ) ;
        }

        // Lọc theo loại phòng
        if ( searchModel . RoomTypeId . HasValue )
        {
            query = query . Where ( r => r . RoomTypeId == searchModel . RoomTypeId . Value ) ;
        }

        // Lọc theo số lượng khách
        if ( searchModel . Guests . HasValue )
        {
            query = query . Where ( r => r . MaxOccupancy >= searchModel . Guests . Value ) ;
        }

        // Lọc theo giá
        if ( searchModel . MinPrice . HasValue )
        {
            decimal minPrice = searchModel . MinPrice . Value ;
            query = query . Where ( r => ( r . DiscountPrice ?? r . PricePerNight ) >= minPrice ) ;
        }

        if ( searchModel . MaxPrice . HasValue )
        {
            decimal maxPrice = searchModel . MaxPrice . Value ;
            query = query . Where ( r => ( r . DiscountPrice ?? r . PricePerNight ) <= maxPrice ) ;
        }

        // Lọc theo ngày có sẵn (nếu có chỉ định ngày check-in và check-out)
        if ( searchModel . CheckInDate . HasValue && searchModel . CheckOutDate . HasValue )
        {
            DateTime checkIn = searchModel . CheckInDate . Value ;
            DateTime checkOut = searchModel . CheckOutDate . Value ;

            // Lấy các phòng không có booking trong khoảng thời gian đó
            var bookedRoomIds = await _bookingRepository . FindAll ( b =>
                                                             ( b . CheckInDate <= checkOut &&
                                                               b . CheckOutDate >= checkIn ) &&
                                                             ( b . BookingStatus == BookingStatus . Pending ||
                                                               b . BookingStatus == BookingStatus . Confirmed ||
                                                               b . BookingStatus == BookingStatus . CheckedIn ) )
                                                         . Select ( b => b . RoomId )
                                                         . Distinct ( )
                                                         . ToListAsync ( ) ;

            if ( bookedRoomIds . Any ( ) )
            {
                query = query . Where ( r => ! bookedRoomIds . Contains ( r . Id ) ) ;
            }
        }

        // Lọc theo tiện nghi
        if ( searchModel . AmenityIds != null && searchModel . AmenityIds . Any ( ) )
        {
            foreach ( var amenityId in searchModel . AmenityIds )
            {
                query = query . Where ( r => r . RoomAmenities . Any ( ra => ra . AmenityId == amenityId ) ) ;
            }
        }

        // Thực hiện truy vấn và chuyển đổi sang ViewModel
        var rooms = await query
                          . Select ( r => new RoomListViewModel
                          {
                              Id = r . Id ,
                              RoomNumber = r . RoomNumber ,
                              Name = r . Name ,
                              MaxOccupancy = r . MaxOccupancy ,
                              AreaInSquareMeters = r . AreaInSquareMeters ,
                              PricePerNight = r . PricePerNight ,
                              DiscountPrice = r . DiscountPrice ,
                              RoomTypeName = r . RoomType . Name ,
                              RoomStatus = r . RoomStatus ,
                              RoomStatusName = r . RoomStatus . ToString ( )
                          } )
                          . ToListAsync ( ) ;

        // Xử lý ảnh đầu tiên cho mỗi phòng
        foreach ( var room in rooms )
        {
            var roomEntity = await _roomRepository . FindByIdAsync ( room . Id ) ;
            if ( ! string . IsNullOrEmpty ( roomEntity . ImageJson ) )
            {
                var images = JsonConvert . DeserializeObject < List < ImageInEntity > > ( roomEntity . ImageJson ) ;
                room . ImageUrl = images . FirstOrDefault ( ) ? . ImageUrl ;
            }
        }

        return rooms ;
    }
}