using Microsoft . AspNetCore . Identity ;

namespace HotelProject.Domain.Entities ;

public class AppUser : IdentityUser<Guid>
{
    public bool IsSystemUser { get; set; }
}