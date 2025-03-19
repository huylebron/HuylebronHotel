namespace HotelProject . Domain . Utility ;

public static class CommonConstants
{
    public class Header
    {
        public const string CurrentUser = "CurrentUser" ;
    }

    public class Permissions
    {
         public const string USER_PERMISSION = "USER_PERMISSION";  // Quyền quản lý người dùng
        public const string ADD_USER_PERMISSION = "ADD_USER_PERMISSION";  // Thêm người dùng mới
        public const string UPDATE_USER_PERMISSION = "UPDATE_USER_PERMISSION";  // Cập nhật thông tin người dùng
        public const string DELETE_USER_PERMISSION = "DELETE_USER_PERMISSION";  // Xóa người dùng
        public const string VIEW_USER_PERMISSION = "VIEW_USER_PERMISSION";  // Xem danh sách người dùng
        
        // Quản lý vai trò
        public const string ROLE_PERMISSION = "ROLE_PERMISSION";  // Quyền quản lý vai trò
        public const string ADD_ROLE_PERMISSION = "ADD_ROLE_PERMISSION";  // Thêm vai trò mới
        public const string UPDATE_ROLE_PERMISSION = "UPDATE_ROLE_PERMISSION";  // Cập nhật vai trò
        public const string DELETE_ROLE_PERMISSION = "DELETE_ROLE_PERMISSION";  // Xóa vai trò
        public const string VIEW_ROLE_PERMISSION = "VIEW_ROLE_PERMISSION";  // Xem danh sách vai trò
        
        // Quản lý khách sạn
        public const string HOTEL_PERMISSION = "HOTEL_PERMISSION";  // Quyền quản lý khách sạn/biệt thự
        public const string ADD_HOTEL_PERMISSION = "ADD_HOTEL_PERMISSION";  // Thêm khách sạn mới
        public const string UPDATE_HOTEL_PERMISSION = "UPDATE_HOTEL_PERMISSION";  // Cập nhật thông tin khách sạn
        public const string DELETE_HOTEL_PERMISSION = "DELETE_HOTEL_PERMISSION";  // Xóa khách sạn
        public const string VIEW_HOTEL_PERMISSION = "VIEW_HOTEL_PERMISSION";  // Xem danh sách khách sạn
        
        // Quản lý loại phòng
        public const string ROOMTYPE_PERMISSION = "ROOMTYPE_PERMISSION";  // Quyền quản lý loại phòng
        public const string ADD_ROOMTYPE_PERMISSION = "ADD_ROOMTYPE_PERMISSION";  // Thêm loại phòng mới
        public const string UPDATE_ROOMTYPE_PERMISSION = "UPDATE_ROOMTYPE_PERMISSION";  // Cập nhật loại phòng
        public const string DELETE_ROOMTYPE_PERMISSION = "DELETE_ROOMTYPE_PERMISSION";  // Xóa loại phòng
        public const string VIEW_ROOMTYPE_PERMISSION = "VIEW_ROOMTYPE_PERMISSION";  // Xem danh sách loại phòng
        
        // Quản lý phòng
        public const string ROOM_PERMISSION = "ROOM_PERMISSION";  // Quyền quản lý phòng
        public const string ADD_ROOM_PERMISSION = "ADD_ROOM_PERMISSION";  // Thêm phòng mới
        public const string UPDATE_ROOM_PERMISSION = "UPDATE_ROOM_PERMISSION";  // Cập nhật thông tin phòng
        public const string DELETE_ROOM_PERMISSION = "DELETE_ROOM_PERMISSION";  // Xóa phòng
        public const string VIEW_ROOM_PERMISSION = "VIEW_ROOM_PERMISSION";  // Xem danh sách phòng
        
        // Quản lý tiện nghi
        public const string AMENITY_PERMISSION = "AMENITY_PERMISSION";  // Quyền quản lý tiện nghi
        public const string ADD_AMENITY_PERMISSION = "ADD_AMENITY_PERMISSION";  // Thêm tiện nghi mới
        public const string UPDATE_AMENITY_PERMISSION = "UPDATE_AMENITY_PERMISSION";  // Cập nhật tiện nghi
        public const string DELETE_AMENITY_PERMISSION = "DELETE_AMENITY_PERMISSION";  // Xóa tiện nghi
        public const string VIEW_AMENITY_PERMISSION = "VIEW_AMENITY_PERMISSION";  // Xem danh sách tiện nghi
        
        // Quản lý đặt phòng
        public const string BOOKING_PERMISSION = "BOOKING_PERMISSION";  // Quyền quản lý đặt phòng
        public const string ADD_BOOKING_PERMISSION = "ADD_BOOKING_PERMISSION";  // Thêm đơn đặt phòng mới
        public const string UPDATE_BOOKING_PERMISSION = "UPDATE_BOOKING_PERMISSION";  // Cập nhật đơn đặt phòng
        public const string DELETE_BOOKING_PERMISSION = "DELETE_BOOKING_PERMISSION";  // Xóa đơn đặt phòng
        public const string VIEW_BOOKING_PERMISSION = "VIEW_BOOKING_PERMISSION";  // Xem danh sách đặt phòng
        
        // Quản lý dịch vụ bổ sung
        public const string SERVICE_PERMISSION = "SERVICE_PERMISSION";  // Quyền quản lý dịch vụ
        public const string ADD_SERVICE_PERMISSION = "ADD_SERVICE_PERMISSION";  // Thêm dịch vụ mới
        public const string UPDATE_SERVICE_PERMISSION = "UPDATE_SERVICE_PERMISSION";  // Cập nhật dịch vụ
        public const string DELETE_SERVICE_PERMISSION = "DELETE_SERVICE_PERMISSION";  // Xóa dịch vụ
        public const string VIEW_SERVICE_PERMISSION = "VIEW_SERVICE_PERMISSION";  // Xem danh sách dịch vụ
        
        // Quản lý đánh giá
        public const string REVIEW_PERMISSION = "REVIEW_PERMISSION";  // Quyền quản lý đánh giá
        public const string ADD_REVIEW_PERMISSION = "ADD_REVIEW_PERMISSION";  // Thêm đánh giá mới
        public const string UPDATE_REVIEW_PERMISSION = "UPDATE_REVIEW_PERMISSION";  // Cập nhật đánh giá
        public const string DELETE_REVIEW_PERMISSION = "DELETE_REVIEW_PERMISSION";  // Xóa đánh giá
        public const string VIEW_REVIEW_PERMISSION = "VIEW_REVIEW_PERMISSION";  // Xem danh sách đánh giá
        
        // Quản lý hình ảnh
        public const string IMAGE_PERMISSION = "IMAGE_PERMISSION";  // Quyền quản lý hình ảnh
        public const string ADD_IMAGE_PERMISSION = "ADD_IMAGE_PERMISSION";  // Thêm hình ảnh mới
        public const string UPDATE_IMAGE_PERMISSION = "UPDATE_IMAGE_PERMISSION";  // Cập nhật hình ảnh
        public const string DELETE_IMAGE_PERMISSION = "DELETE_IMAGE_PERMISSION";  // Xóa hình ảnh
        public const string VIEW_IMAGE_PERMISSION = "VIEW_IMAGE_PERMISSION";  // Xem danh sách hình ảnh
        
        // Quản lý báo cáo
        public const string REPORT_PERMISSION = "REPORT_PERMISSION";  // Quyền xem báo cáo
        public const string BOOKING_REPORT_PERMISSION = "BOOKING_REPORT_PERMISSION";  // Xem báo cáo đặt phòng
        public const string REVENUE_REPORT_PERMISSION = "REVENUE_REPORT_PERMISSION";  // Xem báo cáo doanh thu
         
        
    }
}

