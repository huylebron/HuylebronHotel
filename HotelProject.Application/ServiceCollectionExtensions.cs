using HotelProject . Application . Services ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using Microsoft . Extensions . DependencyInjection ;

namespace HotelProject.Application ;

public static class ServiceCollectionExtensions
{
    public static void AddServicesApplication(this IServiceCollection services)
    {
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
       // services.AddScoped<IRoomService, RoomService>();
       // services.AddScoped<IAmenityService, AmenityService>();
       // services.AddScoped<IAdditionalServiceService, AdditionalServiceService>();
     //   services.AddScoped<IBookingService, BookingService>();
      //  services.AddScoped<IReviewService, ReviewService>();
     //   services.AddScoped<IDashboardService, DashboardService>();
       
    }

  
}