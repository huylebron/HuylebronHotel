using Demo . Application . Services ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using Microsoft . Extensions . DependencyInjection ;

namespace HotelProject.Application ;

public static class ServiceCollectionExtensions
{
    public static void AddServicesApplication(this IServiceCollection services)
    {
        //services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IUserService, UserService>();
       
    }
}