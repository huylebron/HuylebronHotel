
using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Domain . Model . Files ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HotelProject .Infrastructure
{

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _webRootFolder;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _webRootFolder = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrEmpty(_webRootFolder))
            {
                _webRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                Directory.CreateDirectory(_webRootFolder);
            }
        }
        public async Task<FileInfoModel> UploadFile(IFormFile file, string folderName)
        {
            // Kiểm tra thư mục không null
            if (string.IsNullOrEmpty(_webRootFolder))
            {
                throw new DirectoryNotFoundException("Thư mục wwwroot không tồn tại .");
            }
    
            // Kiểm tra file không null
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File ko null");
            }
    
            // Kiểm tra folderName không null
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = "uploads"; // Folder mặc định 
            }
    
            string directory = Path.Combine(_webRootFolder, folderName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
    
            var fileName = $"{Guid.NewGuid()}-{file.FileName}";
            var filePath = $"{folderName}/{fileName}";
            await using var stream = new FileStream(Path.Combine(directory, fileName), FileMode.Create);
            await file.CopyToAsync(stream);
            return new FileInfoModel(file.FileName, filePath);
        }

        public void Delete(string imageUrl)
        {
            string filePath = Path.Combine(_webRootFolder, imageUrl);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            File.Delete(filePath);
        }
    }
}
