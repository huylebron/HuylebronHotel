using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Persistence ;
using Microsoft . EntityFrameworkCore ;

namespace HotelProject.Api ;

public class InitializeTestData
{
    public static async Task SeedTestData ( IApplicationBuilder app ) {
        using var serviceScope = app . ApplicationServices . GetService < IServiceScopeFactory > ( ) . CreateScope ( ) ;
        var dbContext = serviceScope . ServiceProvider . GetRequiredService < ApplicationDbContext > ( ) ;

        // Kiểm tra xem đã có dữ liệu chưa
        if ( await dbContext . Hotels . AnyAsync ( ) )
            return ;

        // 1. Tạo khách sạn mẫu
        var hotels = new List < Hotel >
        {
            new Hotel
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Khách sạn Seaside Luxury" ,
                Description = "Khách sạn 5 sao với view biển tuyệt đẹp" ,
                Address = "123 Đường Biển, Nha Trang" ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new Hotel
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Khách sạn Mountain View" ,
                Description = "Resort nghỉ dưỡng sang trọng trên núi" ,
                Address = "45 Đường Núi, Đà Lạt" ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            }
        } ;

        dbContext . Hotels . AddRange ( hotels ) ;
        await dbContext . SaveChangesAsync ( ) ;

        // 2. Tạo loại phòng mẫu
        var roomTypes = new List < RoomType >
        {
            new RoomType
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Phòng Deluxe" ,
                Description = "Phòng sang trọng với đầy đủ tiện nghi" ,
                HotelId = hotels [ 0 ] . Id ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new RoomType
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Phòng Suite" ,
                Description = "Phòng cao cấp với phòng khách riêng biệt" ,
                HotelId = hotels [ 0 ] . Id ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new RoomType
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Phòng Standard" ,
                Description = "Phòng tiêu chuẩn tiện nghi cơ bản" ,
                HotelId = hotels [ 1 ] . Id ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            }
        } ;

        dbContext . RoomTypes . AddRange ( roomTypes ) ;
        await dbContext . SaveChangesAsync ( ) ;

        // 3. Tạo tiện nghi mẫu
        var amenities = new List < Amenity >
        {
            new Amenity
            {
                Id = Guid . NewGuid ( ) ,
                Name = "WiFi miễn phí" ,
                Description = "WiFi tốc độ cao miễn phí" ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new Amenity
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Bể bơi" ,
                Description = "Bể bơi vô cực ngoài trời" ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new Amenity
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Minibar" ,
                Description = "Minibar đầy đủ đồ uống" ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new Amenity
            {
                Id = Guid . NewGuid ( ) ,
                Name = "TV màn hình phẳng" ,
                Description = "TV LED 55 inch" ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            }
        } ;

        dbContext . Amenities . AddRange ( amenities ) ;
        await dbContext . SaveChangesAsync ( ) ;

        // 4. Tạo phòng mẫu
        var rooms = new List < Room >
        {
            new Room
            {
                Id = Guid . NewGuid ( ) ,
                RoomNumber = "101" ,
                Name = "Deluxe King" ,
                Description = "Phòng deluxe với giường king size" ,
                MaxOccupancy = 2 ,
                AreaInSquareMeters = 35 ,
                PricePerNight = 2000000 ,
                RoomTypeId = roomTypes [ 0 ] . Id ,
                RoomStatus = RoomStatus . Available ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new Room
            {
                Id = Guid . NewGuid ( ) ,
                RoomNumber = "102" ,
                Name = "Deluxe Twin" ,
                Description = "Phòng deluxe với 2 giường đơn" ,
                MaxOccupancy = 2 ,
                AreaInSquareMeters = 35 ,
                PricePerNight = 1800000 ,
                RoomTypeId = roomTypes [ 0 ] . Id ,
                RoomStatus = RoomStatus . Available ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new Room
            {
                Id = Guid . NewGuid ( ) ,
                RoomNumber = "201" ,
                Name = "Suite King" ,
                Description = "Phòng suite với giường king size và phòng khách" ,
                MaxOccupancy = 3 ,
                AreaInSquareMeters = 50 ,
                PricePerNight = 3500000 ,
                RoomTypeId = roomTypes [ 1 ] . Id ,
                RoomStatus = RoomStatus . Available ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new Room
            {
                Id = Guid . NewGuid ( ) ,
                RoomNumber = "301" ,
                Name = "Standard Twin" ,
                Description = "Phòng tiêu chuẩn với 2 giường đơn" ,
                MaxOccupancy = 2 ,
                AreaInSquareMeters = 25 ,
                PricePerNight = 1000000 ,
                RoomTypeId = roomTypes [ 2 ] . Id ,
                RoomStatus = RoomStatus . Available ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new Room
            {
                Id = Guid . NewGuid ( ) ,
                RoomNumber = "302" ,
                Name = "Standard Double" ,
                Description = "Phòng tiêu chuẩn với giường đôi" ,
                MaxOccupancy = 2 ,
                AreaInSquareMeters = 25 ,
                PricePerNight = 1200000 ,
                RoomTypeId = roomTypes [ 2 ] . Id ,
                RoomStatus = RoomStatus . Available ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            }
        } ;

        dbContext . Rooms . AddRange ( rooms ) ;
        await dbContext . SaveChangesAsync ( ) ;

        // 5. Gán tiện nghi cho phòng
        var roomAmenities = new List < RoomAmenity > ( ) ;

        // Phòng 1 có WiFi và TV
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 0 ] . Id , AmenityId = amenities [ 0 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 0 ] . Id , AmenityId = amenities [ 3 ] . Id } ) ;

        // Phòng 2 có WiFi, TV và minibar
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 1 ] . Id , AmenityId = amenities [ 0 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 1 ] . Id , AmenityId = amenities [ 2 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 1 ] . Id , AmenityId = amenities [ 3 ] . Id } ) ;

        // Phòng 3 (Suite) có tất cả tiện nghi
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 2 ] . Id , AmenityId = amenities [ 0 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 2 ] . Id , AmenityId = amenities [ 1 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 2 ] . Id , AmenityId = amenities [ 2 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 2 ] . Id , AmenityId = amenities [ 3 ] . Id } ) ;

        // Phòng 4 và 5 có WiFi và TV
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 3 ] . Id , AmenityId = amenities [ 0 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 3 ] . Id , AmenityId = amenities [ 3 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 4 ] . Id , AmenityId = amenities [ 0 ] . Id } ) ;
        roomAmenities . Add ( new RoomAmenity
            { Id = Guid . NewGuid ( ) , RoomId = rooms [ 4 ] . Id , AmenityId = amenities [ 3 ] . Id } ) ;

        dbContext . RoomAmenities . AddRange ( roomAmenities ) ;
        await dbContext . SaveChangesAsync ( ) ;

        // 6. Tạo dịch vụ bổ sung
        var services = new List < AdditionalService >
        {
            new AdditionalService
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Bữa sáng" ,
                Description = "Bữa sáng buffet" ,
                Price = 200000 ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new AdditionalService
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Dịch vụ đưa đón sân bay" ,
                Description = "Xe đưa đón sân bay" ,
                Price = 500000 ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            } ,
            new AdditionalService
            {
                Id = Guid . NewGuid ( ) ,
                Name = "Dịch vụ spa" ,
                Description = "Spa và massage" ,
                Price = 800000 ,
                Status = EntityStatus . Active ,
                CreatedDate = DateTime . Now
            }
        } ;

        dbContext . AdditionalServices . AddRange ( services ) ;
        await dbContext . SaveChangesAsync ( ) ;
    }
}