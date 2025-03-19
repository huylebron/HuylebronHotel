using Microsoft . AspNetCore . Http ;

namespace HotelProject.Domain.Exception ;

public class BadRequestException : BaseException
{
    protected BadRequestException(string message)
        : base("Bad Request", message)
    {
        StatusCode = StatusCodes.Status400BadRequest;
    }
    
    
    
}