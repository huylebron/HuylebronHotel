using HotelProject . Api . Controllers . Bases ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Users ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject.Api.Controllers.Public ;

public class NoAuthController : NoAuthorizeController
{
    private readonly IUserService _userService;
    public NoAuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<AuthorizedResponseModel> Login([FromBody] LoginViewModel model)
    {
        var result = await _userService.Login(model);
        return result;
    }

    [HttpPost]
    [Route("register-customer")]
    public async Task<ResponseResult> RegisterCustomer([FromBody] RegisterUserViewModel model)
    {
        var result = await _userService.RegisterCustomer(model);
        return result;
    }
    
}