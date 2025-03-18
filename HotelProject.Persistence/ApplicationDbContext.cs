using HotelProject . Domain . Entities ;
using Microsoft . AspNetCore . Identity . EntityFrameworkCore ;
using Microsoft . EntityFrameworkCore ;

namespace HotelProject.Persistence ;

public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    }
    public DbSet<GeneralImage> GeneralImages { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<RoomAmenity> RoomAmenities { get; set; }
    public DbSet<RoomReview> RoomReviews { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<AdditionalService> AdditionalServices { get; set; }
    public DbSet<BookingService> BookingServices { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    
    
    
    
}