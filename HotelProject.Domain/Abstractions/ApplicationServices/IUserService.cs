
using System.Threading.Tasks;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Users ;

namespace HotelProject . Domain . Abstractions .ApplicationServices
{
    public interface IUserService
    {
        #region Commons
        Task<AuthorizedResponseModel> Login(LoginViewModel model);
        Task<UserProfileModel> GetUserProfile(string userName);
        Task<bool> InitializeUserAdminAsync();
        Task<ResponseResult> UpdateUserInfo(UpdateUserInfoViewModel model, UserProfileModel currentUser);
        Task<ResponseResult> ChangePassword(ChangePasswordViewModel model, UserProfileModel currentUser);
        #endregion

        #region Customers
        Task<ResponseResult> RegisterCustomer(RegisterUserViewModel model);
        #endregion

        #region SystemUsers
        Task<ResponseResult> RegisterSystemUser(RegisterUserViewModel model);
        Task<ResponseResult> AssignRoles(AssignRolesViewModel model);

        Task<ResponseResult> RemoveRoles(RemoveRolesViewModel model);

        Task<ResponseResult> AssignPermissions(AssignPermissionsViewModel model);

        Task<PageResult<UserViewModel>> GetUsers(UserSearchQuery query);

        Task<PageResult<RoleViewModel>> GetRoles(RoleSearchQuery query);

        Task<RoleViewModel> GetRoleDetail(Guid roleId);

        Task<ResponseResult> CreateRole(CreateRoleViewModel model);
        Task<ResponseResult> UpdateRole(UpdateRoleViewModel model);
        Task<ResponseResult> DeleteRole(Guid roleId);
        Task<ResponseResult> DeleteUser(Guid roleId);
        #endregion
    }
}
