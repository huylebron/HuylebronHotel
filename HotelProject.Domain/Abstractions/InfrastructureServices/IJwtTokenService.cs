using System . Security . Claims ;

namespace HotelProject.Domain.Abstractions.InfrastructureServices ;

public interface IJwtTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

}