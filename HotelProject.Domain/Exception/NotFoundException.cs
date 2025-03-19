using Microsoft . AspNetCore . Http ;

namespace HotelProject.Domain.Exception ;

public abstract class NotFoundException : BaseException
{
    protected NotFoundException(string message)
        : base("Not Found", message)
    {
        StatusCode = StatusCodes.Status404NotFound;
    }
}
