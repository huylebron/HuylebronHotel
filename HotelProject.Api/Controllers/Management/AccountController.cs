using HotelProject . Api . Controllers . Bases ;
using HotelProject . Api . Filters ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Users ;
using HotelProject . Domain . Utility ;
using Microsoft . AspNetCore . Mvc ;

namespace HotelProject.Api.Controllers.Management ;

public class AccountController : AuthorizeController
{
    
    private readonly IUserService _userService;
public AccountController(IUserService userService)
{
    _userService = userService;
}

#region user
[HttpPut]
[Route("update-info")]
public async Task<ResponseResult> UpdateUserInfo([FromBody] UpdateUserInfoViewModel model)
{
    var result = await _userService.UpdateUserInfo(model, CurrentUser);
    return result;
}


[HttpPut]
[Route("change-password")]
public async Task<ResponseResult> ChangePassword([FromBody] ChangePasswordViewModel model)
{
    var result = await _userService.ChangePassword(model, CurrentUser);
    return result;
}

[Permission(CommonConstants.Permissions.ADD_USER_PERMISSION)]
[HttpPost]
[Route("register-system-user")]
public async Task<ResponseResult> RegisterSystemUser([FromBody] RegisterUserViewModel model)
{
    var result = await _userService.RegisterSystemUser(model);
    return result;
}



[Permission(CommonConstants.Permissions.VIEW_USER_PERMISSION)]
[HttpPost]
[Route("get-users")]
public async Task<PageResult<UserViewModel>> GetUsers([FromBody] UserSearchQuery model)
{
    var result = await _userService.GetUsers(model);
    return result;
}

[Permission(CommonConstants.Permissions.DELETE_USER_PERMISSION)]
[HttpDelete]
[Route("delete-user")]
public async Task<ResponseResult> DeletUser(Guid userId)
{
    var result = await _userService.DeleteUser(userId);
    return result;
}
#endregion

#region role
[Permission(CommonConstants.Permissions.VIEW_ROLE_PERMISSION)]
[HttpPost]
[Route("get-roles")]
public async Task<PageResult<RoleViewModel>> GetRoles([FromBody] RoleSearchQuery model)
{
    var result = await _userService.GetRoles(model);
    return result;
}

[Permission(CommonConstants.Permissions.VIEW_ROLE_PERMISSION)]
[HttpGet]
[Route("get-role-detail")]
public async Task<RoleViewModel> GetRoleDetail(Guid roleId)
{
    var result = await _userService.GetRoleDetail(roleId);
    return result;
}

[Permission(CommonConstants.Permissions.VIEW_ROLE_PERMISSION)]
[HttpGet]
[Route("create-role")]
public async Task<RoleViewModel> CreateRole(Guid roleId)
{
    var result = await _userService.GetRoleDetail(roleId);
    return result;
}

[Permission(CommonConstants.Permissions.ADD_ROLE_PERMISSION)]
[HttpPost]
[Route("add-role")]
public async Task<ResponseResult> CreateRole([FromBody] CreateRoleViewModel model)
{
    var result = await _userService.CreateRole(model);
    return result;
}

[Permission(CommonConstants.Permissions.UPDATE_ROLE_PERMISSION)]
[HttpPost]
[Route("update-role")]
public async Task<ResponseResult> UpdateRole([FromBody] UpdateRoleViewModel model)
{
    var result = await _userService.UpdateRole(model);
    return result;
}

[Permission(CommonConstants.Permissions.DELETE_ROLE_PERMISSION)]
[HttpDelete]
[Route("delete-role")]
public async Task<ResponseResult> DeletRole(Guid roleId)
{
    var result = await _userService.DeleteRole(roleId);
    return result;
} 
#endregion

#region permision
[Permission(CommonConstants.Permissions.UPDATE_ROLE_PERMISSION)]
[HttpPost]
[Route("assign-role")]
public async Task<ResponseResult> AssignRole([FromBody] AssignRolesViewModel model)
{
    var result = await _userService.AssignRoles(model);
    return result;
}

[Permission(CommonConstants.Permissions.UPDATE_ROLE_PERMISSION)]
[HttpPost]
[Route("remove-role")]
public async Task<ResponseResult> RemoveRole([FromBody] RemoveRolesViewModel model)
{
    var result = await _userService.RemoveRoles(model);
    return result;
}

[Permission(CommonConstants.Permissions.UPDATE_ROLE_PERMISSION)]
[HttpPost]
[Route("assign-permissions")]
public async Task<ResponseResult> AssignPermissions([FromBody] AssignPermissionsViewModel model)
{
    var result = await _userService.AssignPermissions(model);
    return result;
} 
#endregion
    
}