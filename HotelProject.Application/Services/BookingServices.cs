using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . AdditionalService ;
using HotelProject . Domain . Model . Booking ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Payment ;
using HotelProject . Domain . Model . Users ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Logging ;

namespace HotelProject . Application . Services ;

public class BookingServices : IBookingService
{
    private readonly IGenericRepository < Booking , Guid > _bookingRepository ;
    private readonly IGenericRepository < Room , Guid > _roomRepository ;
    private readonly IGenericRepository < AdditionalService , Guid > _additionalServiceRepository ;
    private readonly IGenericRepository < BookingService , Guid > _bookingServiceRepository ;
    private readonly IUnitOfWork _unitOfWork ;
    private readonly ILogger < BookingService > _logger ;

    public BookingServices (
        IGenericRepository < Booking , Guid > bookingRepository ,
        IGenericRepository < Room , Guid > roomRepository ,
        IGenericRepository < AdditionalService , Guid > additionalServiceRepository ,
        IGenericRepository < BookingService , Guid > bookingServiceRepository ,
        IUnitOfWork unitOfWork ,
        ILogger < BookingService > logger ) {
        _bookingRepository = bookingRepository ;
        _roomRepository = roomRepository ;
        _additionalServiceRepository = additionalServiceRepository ;
        _bookingServiceRepository = bookingServiceRepository ;
        _unitOfWork = unitOfWork ;
        _logger = logger ;
    }

    // Lấy danh sách các đơn đặt phòng (có phân trang và tìm kiếm)
    public async Task < PageResult < BookingListViewModel > > GetBookings ( SearchQuery query ) {
        var result = new PageResult < BookingListViewModel >
        {
            CurrentPage = query . PageIndex
        } ;

        // Tạo query với các bảng liên quan
        var bookingQuery = _bookingRepository . FindAll (
            includeProperties :
            b => b . User ,
            b => b . Room ,
            b => b . Room . RoomType ,
            b => b . Room . RoomType . Hotel
        ) ;

        // Lọc theo trạng thái active nếu cần
        if ( query . DisplayActiveItem )
        {
            bookingQuery = bookingQuery . Where ( s => s . Status == EntityStatus . Active ) ;
        }

        // Tìm kiếm theo từ khóa
        if ( ! string . IsNullOrEmpty ( query . Keyword ) )
        {
            bookingQuery = bookingQuery . Where ( s =>
                s . User . UserName . Contains ( query . Keyword ) ||
                s . Room . Name . Contains ( query . Keyword ) ||
                s . Room . RoomNumber . Contains ( query . Keyword ) ||
                ( s . SpecialRequests != null && s . SpecialRequests . Contains ( query . Keyword ) )
            ) ;
        }

        // Đếm tổng số bản ghi
        result . TotalCount = await bookingQuery . CountAsync ( ) ;

        // Lấy dữ liệu phân trang
        var bookings = await bookingQuery
                             . OrderByDescending ( s => s . CreatedDate ) // Đơn mới nhất hiển thị trước
                             . Skip ( query . SkipNo )
                             . Take ( query . TakeNo )
                             . Select ( s => new BookingListViewModel
                             {
                                 Id = s . Id ,
                                 UserName = s . User . UserName ,
                                 RoomName = s . Room . Name ,
                                 RoomNumber = s . Room . RoomNumber ,
                                 HotelName = s . Room . RoomType . Hotel . Name ,
                                 CheckInDate = s . CheckInDate ,
                                 CheckOutDate = s . CheckOutDate ,
                                 NumberOfGuests = s . NumberOfGuests ,
                                 TotalAmount = s . TotalAmount ,
                                 BookingStatus = s . BookingStatus ,
                                 BookingStatusName = s . BookingStatus . ToString ( ) ,
                                 PaymentStatus = s . PaymentStatus ,
                                 PaymentStatusName = s . PaymentStatus . ToString ( )
                             } )
                             . ToListAsync ( ) ;

        result . Data = bookings ;
        return result ;
    }

    // Lấy thông tin chi tiết một đơn đặt phòng
    public async Task < BookingDetailViewModel > GetBookingDetail ( Guid bookingId ) {
        // Lấy thông tin đặt phòng với các bảng liên quan
        var booking = await _bookingRepository . FindByIdAsync ( bookingId ,
            b => b . User ,
            b => b . Room ,
            b => b . Room . RoomType ,
            b => b . Room . RoomType . Hotel ,
            b => b . BookingServices
        ) ;

        if ( booking == null )
        {
            throw new BookingException . BookingNotFoundException ( bookingId ) ;
        }

        // Lấy thông tin dịch vụ bổ sung
        var serviceIds = booking . BookingServices ? . Select ( bs => bs . ServiceId ) . ToList ( ) ??
                         new List < Guid > ( ) ;
        var services = await _additionalServiceRepository . FindAll ( s => serviceIds . Contains ( s . Id ) )
                                                          . ToListAsync ( ) ;

        // Tạo danh sách dịch vụ đã đặt
        var bookingServices = booking . BookingServices ? . Select ( bs =>
        {
            var service = services . FirstOrDefault ( s => s . Id == bs . ServiceId ) ;
            return new BookingServiceViewModel
            {
                ServiceId = bs . ServiceId ,
                ServiceName = service ? . Name ,
                ServicePrice = bs . Price
            } ;
        } ) . ToList ( ) ?? new List < BookingServiceViewModel > ( ) ;

        // Tạo đối tượng kết quả
        var result = new BookingDetailViewModel
        {
            Id = booking . Id ,
            UserName = booking . User . UserName ,
            UserEmail = booking . User . Email ,
            RoomName = booking . Room . Name ,
            RoomNumber = booking . Room . RoomNumber ,
            HotelName = booking . Room . RoomType . Hotel . Name ,
            HotelAddress = booking . Room . RoomType . Hotel . Address ,
            CheckInDate = booking . CheckInDate ,
            CheckOutDate = booking . CheckOutDate ,
            NumberOfGuests = booking . NumberOfGuests ,
            RoomPrice = booking . Room . PricePerNight ,
            TotalAmount = booking . TotalAmount ,
            SpecialRequests = booking . SpecialRequests ,
            BookingStatus = booking . BookingStatus ,
            BookingStatusName = booking . BookingStatus . ToString ( ) ,
            PaymentMethod = booking . PaymentMethod ,
            PaymentMethodName = booking . PaymentMethod . ToString ( ) ,
            PaymentStatus = booking . PaymentStatus ,
            PaymentStatusName = booking . PaymentStatus . ToString ( ) ,
            Services = bookingServices
        } ;

        return result ;
    }

    // Tạo đơn đặt phòng mới
    public async Task < ResponseResult > CreateBooking ( BookingCreateViewModel model , UserProfileModel currentUser ) {
        // Bước 1: Kiểm tra tính hợp lệ của thông tin đặt phòng

        // Kiểm tra thời gian check-in/check-out
        if ( model . CheckInDate >= model . CheckOutDate )
        {
            throw new BookingException . InvalidDateRangeException ( ) ;
        }

        // Kiểm tra phòng có tồn tại không
        var room = await _roomRepository . FindByIdAsync ( model . RoomId ) ;
        if ( room == null )
        {
            throw new RoomException . RoomNotFoundException ( model . RoomId ) ;
        }

        // Kiểm tra phòng có khả dụng không
        if ( room . RoomStatus != RoomStatus . Available )
        {
            throw new RoomException . RoomNotAvailableException ( model . RoomId ) ;
        }

        // Kiểm tra phòng đã được đặt trong khoảng thời gian này chưa
        var existingBooking = await _bookingRepository . FindAll (
            b => b . RoomId == model . RoomId &&
                 b . Status == EntityStatus . Active &&
                 b . BookingStatus != BookingStatus . Cancelled &&
                 ( ( b . CheckInDate <= model . CheckInDate && b . CheckOutDate > model . CheckInDate ) ||
                   ( b . CheckInDate < model . CheckOutDate && b . CheckOutDate >= model . CheckOutDate ) ||
                   ( b . CheckInDate >= model . CheckInDate && b . CheckOutDate <= model . CheckOutDate ) )
        ) . FirstOrDefaultAsync ( ) ;

        if ( existingBooking != null )
        {
            throw new BookingException . RoomAlreadyBookedException ( model . RoomId ) ;
        }

        // Bước 2: Tính toán chi phí đặt phòng

        // Tính số ngày
        int numberOfDays = ( model . CheckOutDate - model . CheckInDate ) . Days ;

        // Tính tổng tiền phòng (ưu tiên giá khuyến mãi nếu có)
        decimal roomTotal = room . DiscountPrice . HasValue && room . DiscountPrice . Value > 0
            ? room . DiscountPrice . Value * numberOfDays
            : room . PricePerNight * numberOfDays ;

        // Bước 3: Tạo đơn đặt phòng mới
        var newBooking = new Booking
        {
            Id = Guid . NewGuid ( ) ,
            UserId = currentUser . UserId ,
            RoomId = model . RoomId ,
            CheckInDate = model . CheckInDate ,
            CheckOutDate = model . CheckOutDate ,
            NumberOfGuests = model . NumberOfGuests ,
            TotalAmount = roomTotal , // Sẽ cập nhật sau khi thêm dịch vụ
            SpecialRequests = model . SpecialRequests ,
            BookingStatus = BookingStatus . Pending ,
            PaymentMethod = model . PaymentMethod ,
            PaymentStatus = PaymentStatus . Pending ,
            CreatedBy = currentUser . UserId ,
            CreatedDate = DateTime . Now ,
            Status = EntityStatus . Active
        } ;

        // Bước 4: Xử lý dịch vụ bổ sung nếu có
        decimal serviceTotal = 0 ;
        List < BookingService > bookingServices = new List < BookingService > ( ) ;

        if ( model . ServiceIds != null && model . ServiceIds . Any ( ) )
        {
            var services = await _additionalServiceRepository
                                 . FindAll ( s => model . ServiceIds . Contains ( s . Id ) )
                                 . ToListAsync ( ) ;

            foreach ( var service in services )
            {
                var bookingService = new BookingService
                {
                    Id = Guid . NewGuid ( ) ,
                    BookingId = newBooking . Id ,
                    ServiceId = service . Id ,
                    Quantity = 1 , // Mặc định số lượng là 1
                    Price = service . Price ,
                    CreatedBy = currentUser . UserId ,
                    CreatedDate = DateTime . Now ,
                    Status = EntityStatus . Active
                } ;

                bookingServices . Add ( bookingService ) ;
                serviceTotal += service . Price ;
            }
        }

        // Cập nhật tổng tiền bao gồm cả dịch vụ
        newBooking . TotalAmount += serviceTotal ;

        // Bước 5: Lưu dữ liệu vào database
        try
        {
            await _unitOfWork . BeginTransactionAsync ( ) ;

            // Lưu booking
            _bookingRepository . Add ( newBooking ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;

            // Lưu booking service nếu có
            if ( bookingServices . Any ( ) )
            {
                _bookingServiceRepository . AddRange ( bookingServices ) ;
                await _unitOfWork . SaveChangesAsync ( ) ;
            }

            // Cập nhật trạng thái phòng
            room . RoomStatus = RoomStatus . Occupied ;
            _roomRepository . Update ( room ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;

            await _unitOfWork . CommitAsync ( ) ;
            return ResponseResult . Success ( "Đặt phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            await _unitOfWork . RollbackAsync ( ) ;
            _logger . LogError ( ex , "Lỗi khi tạo đơn đặt phòng" ) ;
            throw new BookingException . CreateBookingException ( ) ;
        }
    }

    // Cập nhật trạng thái đơn đặt phòng
    public async Task < ResponseResult > UpdateBookingStatus ( BookingUpdateStatusViewModel model ,
        UserProfileModel currentUser ) {
        // Bước 1: Tìm đơn đặt phòng
        var booking = await _bookingRepository . FindByIdAsync ( model . Id , b => b . Room ) ;
        if ( booking == null )
        {
            throw new BookingException . BookingNotFoundException ( model . Id ) ;
        }

        // Bước 2: Cập nhật trạng thái
        booking . BookingStatus = model . BookingStatus ;
        booking . PaymentStatus = model . PaymentStatus ;
        booking . UpdatedBy = currentUser . UserId ;
        booking . UpdatedDate = DateTime . Now ;

        // Bước 3: Cập nhật trạng thái phòng dựa trên trạng thái booking
        var room = booking . Room ;
        if ( model . BookingStatus == BookingStatus . CheckedIn )
        {
            room . RoomStatus = RoomStatus . Occupied ;
        }
        else if ( model . BookingStatus == BookingStatus . CheckedOut ||
                  model . BookingStatus == BookingStatus . Cancelled )
        {
            room . RoomStatus = RoomStatus . Cleaning ; // Sau khi trả phòng cần dọn dẹp
        }

        // Bước 4: Lưu thay đổi vào database
        try
        {
            await _unitOfWork . BeginTransactionAsync ( ) ;

            _bookingRepository . Update ( booking ) ;
            _roomRepository . Update ( room ) ;

            await _unitOfWork . SaveChangesAsync ( ) ;
            await _unitOfWork . CommitAsync ( ) ;

            return ResponseResult . Success ( "Cập nhật trạng thái đặt phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            await _unitOfWork . RollbackAsync ( ) ;
            _logger . LogError ( ex , "Lỗi khi cập nhật trạng thái đặt phòng với ID: {BookingId}" , model . Id ) ;
            throw new BookingException . UpdateBookingException ( model . Id ) ;
        }
    }

    // Hủy đơn đặt phòng
    public async Task < ResponseResult > CancelBooking ( Guid bookingId , UserProfileModel currentUser ) {
        // Bước 1: Tìm đơn đặt phòng
        var booking = await _bookingRepository . FindByIdAsync ( bookingId , b => b . Room ) ;
        if ( booking == null )
        {
            throw new BookingException . BookingNotFoundException ( bookingId ) ;
        }

        // Bước 2: Kiểm tra điều kiện hủy
        // Chỉ cho phép hủy booking ở trạng thái Pending hoặc Confirmed
        if ( booking . BookingStatus != BookingStatus . Pending &&
             booking . BookingStatus != BookingStatus . Confirmed )
        {
            throw new BookingException . UpdateBookingException ( bookingId ) ;
        }

        // Bước 3: Cập nhật trạng thái
        booking . BookingStatus = BookingStatus . Cancelled ;
        booking . UpdatedBy = currentUser . UserId ;
        booking . UpdatedDate = DateTime . Now ;

        // Bước 4: Cập nhật trạng thái phòng về Available
        var room = booking . Room ;
        room . RoomStatus = RoomStatus . Available ;

        // Bước 5: Lưu thay đổi vào database
        try
        {
            await _unitOfWork . BeginTransactionAsync ( ) ;

            _bookingRepository . Update ( booking ) ;
            _roomRepository . Update ( room ) ;

            await _unitOfWork . SaveChangesAsync ( ) ;
            await _unitOfWork . CommitAsync ( ) ;

            return ResponseResult . Success ( "Hủy đặt phòng thành công" ) ;
        }
        catch ( Exception ex )
        {
            await _unitOfWork . RollbackAsync ( ) ;
            _logger . LogError ( ex , "Lỗi khi hủy đặt phòng với ID: {BookingId}" , bookingId ) ;
            throw new BookingException . UpdateBookingException ( bookingId ) ;
        }
    }

    // Xử lý thanh toán (phiên bản đơn giản để demo)
    public async Task < ResponseResult >
        ProcessPayment ( PaymentRequestViewModel model , UserProfileModel currentUser ) {
        // Bước 1: Kiểm tra đơn đặt phòng
        var booking = await _bookingRepository . FindByIdAsync ( model . BookingId ) ;
        if ( booking == null )
        {
            throw new BookingException . BookingNotFoundException ( model . BookingId ) ;
        }

        // Bước 2: Kiểm tra nếu đã thanh toán
        if ( booking . PaymentStatus == PaymentStatus . Paid )
        {
            return ResponseResult . Success ( "Đơn đặt phòng này đã được thanh toán rồi" ) ;
        }

        // Bước 3: Mô phỏng xử lý thanh toán
        bool paymentSuccess = false ;
        string message = "" ;

        // Logic mô phỏng thanh toán đơn giản
        switch ( model . PaymentMethod )
        {
            case PaymentMethod . Cash :
                // Tiền mặt luôn Pending vì cần xác nhận thủ công
                paymentSuccess = true ;
                message = "Đang chờ xác nhận thanh toán tiền mặt" ;
                break ;

            case PaymentMethod . CreditCard :
                // Mô phỏng thanh toán thẻ thành công với thẻ test
                if ( model . CardNumber == "4111111111111111" )
                {
                    paymentSuccess = true ;
                    message = "Thanh toán thẻ thành công" ;
                }
                else
                {
                    paymentSuccess = false ;
                    message = "Thanh toán thẻ thất bại" ;
                }

                break ;

            case PaymentMethod . BankTransfer :
                // Mô phỏng chuyển khoản đang chờ xác nhận
                paymentSuccess = true ;
                message = "Đang chờ xác nhận chuyển khoản" ;
                break ;

            default :
                paymentSuccess = false ;
                message = "Phương thức thanh toán không hỗ trợ" ;
                break ;
        }

        //Cập nhật payment status
        if ( paymentSuccess )
        {
            await _unitOfWork . BeginTransactionAsync ( ) ;

            // Cập nhật trạng thái thanh toán
            booking . PaymentMethod = model . PaymentMethod ;
            booking . PaymentStatus = model . PaymentMethod == PaymentMethod . CreditCard
                ? PaymentStatus . Paid
                : PaymentStatus . Pending ;
            booking . UpdatedBy = currentUser . UserId ;
            booking . UpdatedDate = DateTime . Now ;

            _bookingRepository . Update ( booking ) ;
            await _unitOfWork . SaveChangesAsync ( ) ;
            await _unitOfWork . CommitAsync ( ) ;
        }

   
        return ResponseResult . Success ( message ) ;
    }
}

