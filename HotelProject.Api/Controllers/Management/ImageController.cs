using HotelProject.Api.Controllers.Bases;
using HotelProject.Api.Filters;
using HotelProject.Domain.Abstractions.ApplicationServices;
using HotelProject.Domain.Model.Commons;
using HotelProject.Domain.Model.Images;
using HotelProject.Domain.Utility;
using Microsoft.AspNetCore.Mvc;

namespace HotelProject.Api.Controllers.Management;

[Route("api/[controller]")]
[ApiController]
public class ImageController : AuthorizeController
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    // [Permission(CommonConstants.Permissions.ADD_IMAGE_PERMISSION)]
    [HttpPost]
    [Route("upload-images")]
    public async Task<ResponseResult> UploadImages([FromForm] UploadImageViewModel model)
    {
        var result = await _imageService.UploadImages(model);
        return result;
    }

    [Permission(CommonConstants.Permissions.UPDATE_IMAGE_PERMISSION)]
    [HttpPut]
    [Route("update-image")]
    public async Task<ResponseResult> UpdateImage([FromBody] UpdateImageViewModel model)
    {
        var result = await _imageService.UpdateImage(model);
        return result;
    }

    [Permission(CommonConstants.Permissions.DELETE_IMAGE_PERMISSION)]
    [HttpDelete]
    [Route("delete-image")]
    public async Task<ResponseResult> DeleteImage(Guid imageId)
    {
        var result = await _imageService.DeleteImage(imageId);
        return result;
    }
}