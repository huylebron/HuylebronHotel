
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Data;
using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Users ;
using HotelProject . Domain . Utility ;

namespace Demo.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IGenericRepository<Permission, Guid> _permissionRepository;
        private readonly IGenericRepository<RolePermission, Guid> _rolePermissionRepository;
        private readonly IJwtTokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;


        public UserService(UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IGenericRepository<Permission, Guid> permissionRepository,
            IJwtTokenService tokenService, IGenericRepository<RolePermission, Guid> rolePermissionRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionRepository = permissionRepository;
            _tokenService = tokenService;
            _rolePermissionRepository = rolePermissionRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }


        #region Common
        public async Task<AuthorizedResponseModel> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                throw new UserException.UserNotFoundException();
            }
            var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!checkPassword)
            {
                throw new UserException.PasswordNotCorrectException();
            }

            var claims = new List<Claim>
            {
                new("UserName", user.UserName),
                new(ClaimTypes.Email, user.Email)

            };
            var response = new AuthorizedResponseModel() { JwtToken = _tokenService.GenerateAccessToken(claims) };
            return response;

        }

        public async Task<UserProfileModel> GetUserProfile(string userName)
        {

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UserException.UserNotFoundException();
            }
            var result = new UserProfileModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            if (!user.IsSystemUser)
            {
                return result;
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles == null || !userRoles.Any())
            {
                return result;
            }
            var roles = _roleManager.Roles;
            var permissions = _permissionRepository.FindAll();

            var rolePermissions = _rolePermissionRepository.FindAll();
            var userPermission =
                from r in roles
                join rp in rolePermissions on r.Id equals rp.RoleId
                select new { rp.PermissionCode, r.Name };

            var filerPermissions = userPermission.Where(s => userRoles.Contains(s.Name)).Select(x => x.PermissionCode).ToList();
            result.Permissions = filerPermissions.ToList().DistinctBy(s => s).ToList();
            return result;
        }

        public async Task<ResponseResult> UpdateUserInfo(UpdateUserInfoViewModel model, UserProfileModel currentUser)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(s => s.Id == currentUser.UserId);
            if (user == null)
            {
                throw new UserException.UserNotFoundException();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return ResponseResult.Success("Update user profile success");
            }
            else
            {
                var errors = JsonConvert.SerializeObject(result.Errors);
                throw new UserException.HandleUserException(errors);
            }
        }

        public async Task<ResponseResult> ChangePassword(ChangePasswordViewModel model, UserProfileModel currentUser)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(s => s.Id == currentUser.UserId);
            if (user == null)
            {
                throw new UserException.UserNotFoundException();
            }


            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return ResponseResult.Success("Update user profile success");
            }
            else
            {
                var errors = JsonConvert.SerializeObject(result.Errors);
                throw new UserException.HandleUserException(errors);
            }
        }


        #endregion

        #region Customers

        public async Task<ResponseResult> RegisterCustomer(RegisterUserViewModel model)
        {
            var user = new AppUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNummber,
                IsSystemUser = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return ResponseResult.Success();
            }
            else
            {
                var errors = JsonConvert.SerializeObject(result.Errors);
                throw new UserException.HandleUserException(errors);
            }
        }




        #endregion

        #region  System_Users
        public async Task<ResponseResult> RegisterSystemUser(RegisterUserViewModel model)
        {
            var user = new AppUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNummber,
                IsSystemUser = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return ResponseResult.Success();
            }
            else
            {
                var errors = JsonConvert.SerializeObject(result.Errors);
                throw new UserException.HandleUserException(errors);
            }
        }

        public async Task<ResponseResult> AssignRoles(AssignRolesViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                throw new UserException.UserNotFoundException();
            }

            var result = await _userManager.AddToRolesAsync(user, model.RoleNames);
            if (result.Succeeded)
            {
                return ResponseResult.Success("Assign roles to user success");
            }
            var errors = JsonConvert.SerializeObject(result.Errors);
            throw new UserException.HandleUserException(errors);
        }

        public async Task<ResponseResult> RemoveRoles(RemoveRolesViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                throw new UserException.UserNotFoundException();
            }

            var result = await _userManager.RemoveFromRolesAsync(user, model.RoleNames);
            if (result.Succeeded)
            {
                return ResponseResult.Success();
            }
            var errors = JsonConvert.SerializeObject(result.Errors);
            throw new UserException.HandleUserException(errors);
        }

        public async Task<ResponseResult> AssignPermissions(AssignPermissionsViewModel model)

        {
            var permissions = model.Permissions.Where(s => s.IsInRole).Select(s => new RolePermission()
            {
                Id = Guid.NewGuid(),
                RoleId = model.RoleId,
                PermissionCode = s.PermissionCode,
            }).ToList();
            var currentPermissions = await _rolePermissionRepository.FindAll(s => s.RoleId == model.RoleId).ToListAsync();
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _rolePermissionRepository.RemoveMultiple(currentPermissions);
                _rolePermissionRepository.AddRange(permissions);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return ResponseResult.Success();
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                throw new UserException.HandleUserException("Somethings went wrong");
            }
        }

        public async Task<PageResult<UserViewModel>> GetUsers(UserSearchQuery query)
        {
            var result = new PageResult<UserViewModel>() { CurrentPage = query.PageIndex };
            var users = _userManager.Users.Where(s => s.IsSystemUser == query.IsSystemUser);
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                users = users.Where(s => s.UserName.Contains(query.Keyword)
                                    || s.Email.Contains(query.Keyword)
                                    || s.PhoneNumber.Contains(query.Keyword));
            }
            result.TotalCount = await users.CountAsync();
            result.Data = await users.Select(s => new UserViewModel
            {
                UserId = s.Id,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                UserName = s.UserName,
            }).ToListAsync();
            return result;
        }

        public async Task<PageResult<RoleViewModel>> GetRoles(RoleSearchQuery query)
        {
            var result = new PageResult<RoleViewModel>() { CurrentPage = query.PageIndex };
            var roles = _roleManager.Roles;
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                roles = roles.Where(s => s.Name.Contains(query.Keyword));
            }
            result.TotalCount = await roles.CountAsync();

            result.Data = await roles.Select
                 (s => new RoleViewModel
                 {
                     RoleId = s.Id,
                     RoleName = s.Name
                 }).ToListAsync();
            return result;
        }

        public async Task<RoleViewModel> GetRoleDetail(Guid roleId)
        {
            var roles = _roleManager.Roles;
            var permissions = await _permissionRepository.FindAll().ToListAsync();
            var rolePermission = _rolePermissionRepository.FindAll();

            var role = roles.FirstOrDefault(s => s.Id == roleId);
            if (role == null)
            {
                throw new UserException.RoleNotFoundException();
            }

            var permissionCodesInRole = await rolePermission.Where(s => s.RoleId == roleId).Select(x => x.PermissionCode).ToListAsync();
            var permissionViewModels = permissions.Select(s => new PermissionViewModel
            {
                IsInRole = permissionCodesInRole.Contains(s.Code),
                PermissionName = s.Name,
                PermissionCode = s.Code,
                ParentPermissionCode = s.ParentCode
            }).ToList();

            var usersInRole = (await _userManager.GetUsersInRoleAsync(role.Name)).Select(s => new UserViewModel
            {
                UserId = s.Id,
                PhoneNumber = s.PhoneNumber,
                UserName = s.UserName,
                Email = s.Email
            }).ToList();
            var result = new RoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Permissions = permissionViewModels,
                UsersInRole = usersInRole
            };
            return result;

        }

        public async Task<ResponseResult> CreateRole(CreateRoleViewModel model)

        {
            var role = new AppRole()
            {
                Name = model.RoleName
            };
            await _roleManager.CreateAsync(role);
            return ResponseResult.Success();
        }

        public async Task<ResponseResult> UpdateRole(UpdateRoleViewModel model)

        {
            var role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
            if (role == null)
            {
                throw new UserException.RoleNotFoundException();
            }
            role.Name = model.RoleName;
            await _roleManager.UpdateAsync(role);
            return ResponseResult.Success();
        }

        public async Task<ResponseResult> DeleteRole(Guid roleId)

        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new UserException.RoleNotFoundException();
            }

            await _roleManager.DeleteAsync(role);
            return ResponseResult.Success();
        }

        public async Task<ResponseResult> DeleteUser(Guid userId)

        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new UserException.UserNotFoundException();
            }

            await _userManager.DeleteAsync(user);
            return ResponseResult.Success();
        }

        #endregion

        #region Seeding Data
        public async Task<bool> InitializeUserAdminAsync()
        {
            var userAdmin = _configuration.GetSection("UserAdmin");
            if (userAdmin != null)
            {
                var user = await _userManager.FindByNameAsync(userAdmin["UserName"]);
                if (user == null)
                {
                    var createUser = new AppUser()
                    {
                        UserName = userAdmin["UserName"],
                        Email = userAdmin["Email"],
                        IsSystemUser = true
                    };

                    var createUserResult = await _userManager.CreateAsync(createUser, userAdmin["Password"]);

                    if (!createUserResult.Succeeded)
                    {
                        return false;
                    }

                    var adminRole = new AppRole() { Name = userAdmin["Role"] };
                    var createRoleResult = await _roleManager.CreateAsync(adminRole);
                    if (!createRoleResult.Succeeded)
                    {
                        return false;
                    }

                    var assignRoleResult = await _userManager.AddToRoleAsync(createUser, adminRole.Name);
                    if (!assignRoleResult.Succeeded)
                    {
                        return false;
                    }

                    var listPermissions = new List<Permission>
                    {
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ROLE_PERMISSION,
                            Name = CommonConstants.Permissions.ROLE_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_ROLE_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_ROLE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROLE_PERMISSION,
                            Index = 2
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_ROLE_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_ROLE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROLE_PERMISSION,
                            Index = 3
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_ROLE_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_ROLE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROLE_PERMISSION,
                            Index = 4
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_ROLE_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_ROLE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROLE_PERMISSION,
                            Index = 5
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.USER_PERMISSION,
                            Name = CommonConstants.Permissions.USER_PERMISSION,
                            Index = 1
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_USER_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_USER_PERMISSION,
                            ParentCode = CommonConstants.Permissions.USER_PERMISSION,
                            Index = 2
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_USER_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_USER_PERMISSION,
                            ParentCode = CommonConstants.Permissions.USER_PERMISSION,
                            Index = 3
                        }
                        ,

                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_USER_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_USER_PERMISSION,
                            ParentCode = CommonConstants.Permissions.USER_PERMISSION,
                            Index = 4
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_USER_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_USER_PERMISSION,
                            ParentCode = CommonConstants.Permissions.USER_PERMISSION,
                            Index = 5
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.HOTEL_PERMISSION,
                            Name = CommonConstants.Permissions.HOTEL_PERMISSION,
                            Index = 1
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_HOTEL_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_HOTEL_PERMISSION,
                            ParentCode = CommonConstants.Permissions.VIEW_HOTEL_PERMISSION,
                            Index = 2
                        }
                        ,
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_HOTEL_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_HOTEL_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ADD_HOTEL_PERMISSION,
                            Index = 3
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_HOTEL_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_HOTEL_PERMISSION,
                            ParentCode = CommonConstants.Permissions.HOTEL_PERMISSION,
                            Index = 4
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_HOTEL_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_HOTEL_PERMISSION,
                            ParentCode = CommonConstants.Permissions.HOTEL_PERMISSION,
                            Index = 5
                        },


                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ROOMTYPE_PERMISSION,
                            Name = CommonConstants.Permissions.ROOMTYPE_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_ROOMTYPE_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_ROOMTYPE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROOMTYPE_PERMISSION,
                            Index = 2
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_ROOMTYPE_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_ROOMTYPE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROOMTYPE_PERMISSION,
                            Index = 3
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_ROOMTYPE_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_ROOMTYPE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROOMTYPE_PERMISSION,
                            Index = 4
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_ROOMTYPE_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_ROOMTYPE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROOMTYPE_PERMISSION,
                            Index = 5
                        },

                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ROOM_PERMISSION,
                            Name = CommonConstants.Permissions.ROOM_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_ROOM_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_ROOM_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROOM_PERMISSION,
                            Index = 2
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_ROOM_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_ROOM_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROOM_PERMISSION,
                            Index = 3
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_ROOM_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_ROOM_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROOM_PERMISSION,
                            Index = 4
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_ROOM_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_ROOM_PERMISSION,
                            ParentCode = CommonConstants.Permissions.ROOM_PERMISSION,
                            Index = 5
                        },

                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.AMENITY_PERMISSION,
                            Name = CommonConstants.Permissions.AMENITY_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_AMENITY_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_AMENITY_PERMISSION,
                            ParentCode = CommonConstants.Permissions.AMENITY_PERMISSION,
                            Index = 2
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_AMENITY_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_AMENITY_PERMISSION,
                            ParentCode = CommonConstants.Permissions.AMENITY_PERMISSION,
                            Index = 3
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_AMENITY_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_AMENITY_PERMISSION,
                            ParentCode = CommonConstants.Permissions.AMENITY_PERMISSION,
                            Index = 4
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_AMENITY_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_AMENITY_PERMISSION,
                            ParentCode = CommonConstants.Permissions.AMENITY_PERMISSION,
                            Index = 5
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.BOOKING_PERMISSION,
                            Name = CommonConstants.Permissions.BOOKING_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_BOOKING_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_BOOKING_PERMISSION,
                            ParentCode = CommonConstants.Permissions.BOOKING_PERMISSION,
                            Index = 2
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_BOOKING_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_BOOKING_PERMISSION,
                            ParentCode = CommonConstants.Permissions.BOOKING_PERMISSION,
                            Index = 3
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_BOOKING_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_BOOKING_PERMISSION,
                            ParentCode = CommonConstants.Permissions.BOOKING_PERMISSION,
                            Index = 4
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_BOOKING_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_BOOKING_PERMISSION,
                            ParentCode = CommonConstants.Permissions.BOOKING_PERMISSION,
                            Index = 5
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.SERVICE_PERMISSION,
                            Name = CommonConstants.Permissions.SERVICE_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_SERVICE_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_SERVICE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.SERVICE_PERMISSION,
                            Index = 2
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_SERVICE_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_SERVICE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.SERVICE_PERMISSION,
                            Index = 3
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_SERVICE_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_SERVICE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.SERVICE_PERMISSION,
                            Index = 4
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_SERVICE_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_SERVICE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.SERVICE_PERMISSION,
                            Index = 5
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.REVIEW_PERMISSION,
                            Name = CommonConstants.Permissions.REVIEW_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_REVIEW_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_REVIEW_PERMISSION,
                            ParentCode = CommonConstants.Permissions.REVIEW_PERMISSION,
                            Index = 2
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_REVIEW_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_REVIEW_PERMISSION,
                            ParentCode = CommonConstants.Permissions.REVIEW_PERMISSION,
                            Index = 3
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_REVIEW_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_REVIEW_PERMISSION,
                            ParentCode = CommonConstants.Permissions.REVIEW_PERMISSION,
                            Index = 4
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_REVIEW_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_REVIEW_PERMISSION,
                            ParentCode = CommonConstants.Permissions.REVIEW_PERMISSION,
                            Index = 5
                        },
                         new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.REPORT_PERMISSION,
                            Name = CommonConstants.Permissions.REPORT_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.BOOKING_REPORT_PERMISSION,
                            Name = CommonConstants.Permissions.BOOKING_REPORT_PERMISSION,
                            ParentCode = CommonConstants.Permissions.REPORT_PERMISSION,
                            Index = 2
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.REVENUE_REPORT_PERMISSION,
                            Name = CommonConstants.Permissions.REVENUE_REPORT_PERMISSION,
                            ParentCode = CommonConstants.Permissions.REPORT_PERMISSION,
                            Index = 3
                        },
                        
                        
                        


                         new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.IMAGE_PERMISSION,
                            Name = CommonConstants.Permissions.IMAGE_PERMISSION,
                            Index = 1
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.VIEW_IMAGE_PERMISSION,
                            Name = CommonConstants.Permissions.VIEW_IMAGE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.IMAGE_PERMISSION,
                            Index = 2
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.ADD_IMAGE_PERMISSION,
                            Name = CommonConstants.Permissions.ADD_IMAGE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.IMAGE_PERMISSION,
                            Index = 3
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.UPDATE_IMAGE_PERMISSION,
                            Name = CommonConstants.Permissions.UPDATE_IMAGE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.IMAGE_PERMISSION,
                            Index = 4
                        },
                        new Permission()
                        {
                            Id = Guid.NewGuid(),
                            Code = CommonConstants.Permissions.DELETE_IMAGE_PERMISSION,
                            Name = CommonConstants.Permissions.DELETE_IMAGE_PERMISSION,
                            ParentCode = CommonConstants.Permissions.IMAGE_PERMISSION,
                            Index = 5
                        }
                    };

                    var rolesPermissons = listPermissions.Select(s => new RolePermission()
                    {
                        Id = Guid.NewGuid(),
                        RoleId = adminRole.Id,
                        PermissionCode = s.Code,
                    }).ToList();

                    bool assignPermissionResult = true;

                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();
                        _permissionRepository.AddRange(listPermissions);
                        _rolePermissionRepository.AddRange(rolesPermissons);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        await _unitOfWork.RollbackAsync();
                        assignPermissionResult = false;
                    }

                    if (!assignPermissionResult)
                    {
                        return false;
                    }
                    return true;
                }
            }

            else
            {
                return false;
            }

            return false;
        }
        #endregion
    }
}
