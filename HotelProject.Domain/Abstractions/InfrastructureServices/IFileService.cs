using HotelProject . Domain . Model . Files ;
using Microsoft . AspNetCore . Http ;

namespace HotelProject.Domain.Abstractions.InfrastructureServices ;

public interface IFileService
{
    Task<FileInfoModel> UploadFile(IFormFile file, string folderName);
    void Delete(string imageUrl);
}