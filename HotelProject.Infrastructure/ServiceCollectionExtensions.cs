
using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Infrastructure ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelProject .Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddServicesInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IJwtTokenService, JwtTokenService>();
        services.AddTransient<IFileService, FileService>();
       
        
    }

}