using HotelProject . Domain . Model ;

namespace HotelProject.Domain.Abstractions.ApplicationServices ;

public interface IDashboardService
{
    Task < DashboardViewModel > GetDashboardData ( ) ;
    Task < List < BookingStatsByMonth > > GetBookingStatsByMonth ( int year ) ;
    Task < List < RoomOccupancyRate > > GetRoomOccupancyRates ( ) ;
}