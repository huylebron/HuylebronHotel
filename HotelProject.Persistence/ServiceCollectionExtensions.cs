using HotelProject . Domain ;
using HotelProject . Domain . Entities ;
using Microsoft . AspNetCore . Identity ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Configuration ;
using Microsoft . Extensions . DependencyInjection ;

namespace HotelProject.Persistence ;

public static class ServiceCollectionExtensions
{
    public static void AddSqlServerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(ApplicationDbContext).Assembly.GetName().Name;
        services.AddDbContext<ApplicationDbContext>((option) =>
        {
            option.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"], b => b.MigrationsAssembly(assembly));
        });

        services.AddIdentityCore<AppUser>()
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Lockout.AllowedForNewUsers = true; // Default true
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Default 5
            options.Lockout.MaxFailedAccessAttempts = 3; // Default 5
            options.Lockout.AllowedForNewUsers = true; // Default true
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Default 5
            options.Lockout.MaxFailedAccessAttempts = 3; // Default 5
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
            options.Lockout.AllowedForNewUsers = true;
        });
    }

    public static void AddRepositoryUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped(typeof(IUnitOfWork), typeof(EFUnitOfWork));
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
    }
        
}