using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . AdditionalService ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Images ;
using HotelProject . Domain . Model . Users ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Logging ;
using Newtonsoft . Json ;

namespace HotelProject.Application.Services ;

public class AdditionalServiceService : IAdditionalServiceService
{
     private readonly IGenericRepository<AdditionalService, Guid> _serviceRepository;
    private readonly IGenericRepository<BookingService, Guid> _bookingServiceRepository;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdditionalServiceService> _logger;
    private const string _imageFolder = "serviceImages";

    public AdditionalServiceService(
        IGenericRepository<AdditionalService, Guid> serviceRepository,
        IGenericRepository<BookingService, Guid> bookingServiceRepository,
        IUnitOfWork unitOfWork,
        IFileService fileService,
        ILogger<AdditionalServiceService> logger)
    {
        _serviceRepository = serviceRepository;
        _bookingServiceRepository = bookingServiceRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<PageResult<AdditionalServiceViewModel>> GetServices(SearchQuery query)
    {
        var result = new PageResult<AdditionalServiceViewModel>
        {
            CurrentPage = query.PageIndex
        };

        var serviceQuery = _serviceRepository.FindAll();
        
        // Lọc theo trạng thái nếu cần
        if (query.DisplayActiveItem)
        {
            serviceQuery = serviceQuery.Where(s => s.Status == EntityStatus.Active);
        }
        
        // Lọc theo từ khóa nếu có
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            serviceQuery = serviceQuery.Where(s => s.Name.Contains(query.Keyword) || 
                                              (s.Description != null && s.Description.Contains(query.Keyword)));
        }

        // Đếm tổng số bản ghi thỏa mãn điều kiện query
        result.TotalCount = await serviceQuery.CountAsync();
        
        // Lấy dữ liệu phân trang
        var services = await serviceQuery
                            .OrderBy(s => s.Name)
                            .Skip(query.SkipNo)
                            .Take(query.TakeNo)
                            .Select(s => new AdditionalServiceViewModel
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Description = s.Description,
                                Price = s.Price
                            })
                            .ToListAsync();

        // Xử lý ảnh đầu tiên cho mỗi dịch vụ
        foreach (var service in services)
        {
            var serviceEntity = await _serviceRepository.FindByIdAsync(service.Id);
            if (!string.IsNullOrEmpty(serviceEntity.ImageJson))
            {
                var images = JsonConvert.DeserializeObject<List<ImageInEntity>>(serviceEntity.ImageJson);
                service.ImageUrl = images.FirstOrDefault()?.ImageUrl;
            }
        }

        result.Data = services;
        return result;
    }

    public async Task<AdditionalServiceViewModel> GetServiceDetail(Guid serviceId)
    {
        var service = await _serviceRepository.FindByIdAsync(serviceId);
        if (service == null)
        {
            throw new AdditionalServiceException.ServiceNotFoundException(serviceId);
        }
        
        var result = new AdditionalServiceViewModel
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price
        };
        
        // Xử lý ảnh
        if (!string.IsNullOrEmpty(service.ImageJson))
        {
            var images = JsonConvert.DeserializeObject<List<ImageInEntity>>(service.ImageJson);
            result.ImageUrl = images.FirstOrDefault()?.ImageUrl;
        }

        return result;
    }

    public async Task<ResponseResult> CreateService(AdditionalServiceCreateUpdateViewModel model, UserProfileModel currentUser)
    {
        List<ImageInEntity> serviceImages = new List<ImageInEntity>();
            
        // Xử lý upload ảnh
        if (model.ImageFile != null)
        {
            var uploadedImage = await _fileService.UploadFile(model.ImageFile, _imageFolder);
            serviceImages.Add(new ImageInEntity(
                Guid.NewGuid(),
                uploadedImage.FileName,
                uploadedImage.FilePath
            ));
        }

        var newService = new AdditionalService
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            ImageJson = serviceImages.Any() ? JsonConvert.SerializeObject(serviceImages) : null,
            CreatedBy = currentUser.UserId,
            CreatedDate = DateTime.Now,
            Status = EntityStatus.Active
        };

        try
        {
            _serviceRepository.Add(newService);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("Tạo dịch vụ mới thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo dịch vụ mới");
            throw new AdditionalServiceException.UpdateServiceException(Guid.Empty);
        }
    }

    public async Task<ResponseResult> UpdateService(Guid serviceId, AdditionalServiceCreateUpdateViewModel model, UserProfileModel currentUser)
    {
        var service = await _serviceRepository.FindByIdAsync(serviceId);
        if (service == null)
        {
            throw new AdditionalServiceException.ServiceNotFoundException(serviceId);
        }
        
        service.Name = model.Name;
        service.Description = model.Description;
        service.Price = model.Price;
        service.UpdatedBy = currentUser.UserId;
        service.UpdatedDate = DateTime.Now;

        // Xử lý update ảnh 
        if (model.ImageFile != null)
        {
            List<ImageInEntity> serviceImages = new List<ImageInEntity>();
                
            // Thêm ảnh mới
            var uploadedImage = await _fileService.UploadFile(model.ImageFile, _imageFolder);
            serviceImages.Add(new ImageInEntity(
                Guid.NewGuid(),
                uploadedImage.FileName,
                uploadedImage.FilePath
            ));
                
            service.ImageJson = JsonConvert.SerializeObject(serviceImages);
        }

        try
        {
            _serviceRepository.Update(service);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("Cập nhật dịch vụ thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật dịch vụ với ID: {ServiceId}", serviceId);
            throw new AdditionalServiceException.UpdateServiceException(serviceId);
        }
    }

    public async Task<ResponseResult> DeleteService(Guid serviceId)
    {
        var service = await _serviceRepository.FindByIdAsync(serviceId);
        if (service == null)
        {
            throw new AdditionalServiceException.ServiceNotFoundException(serviceId);
        }
        
        // Kiểm tra xem dịch vụ đã được sử dụng trong đơn đặt phòng nào chưa
        var usedInBookings = await _bookingServiceRepository.FindAll(bs => bs.ServiceId == serviceId).AnyAsync();
        if (usedInBookings)
        {
            return ResponseResult.Fail ( "Không thể xóa dịch vụ đang được sử dụng" );
        }

        try
        {
            _serviceRepository.Remove(service);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("Xóa dịch vụ thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa dịch vụ với ID: {ServiceId}", serviceId);
            throw new AdditionalServiceException.DeleteServiceException(serviceId);
        }
    }

    public async Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model)
    {
        var service = await _serviceRepository.FindByIdAsync(model.Id);
        if (service == null)
        {
            throw new AdditionalServiceException.ServiceNotFoundException(model.Id);
        }

        service.Status = model.Status;
            
        try
        {
            _serviceRepository.Update(service);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("Cập nhật trạng thái dịch vụ thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật trạng thái dịch vụ có ID: {ServiceId}", model.Id);
            throw new AdditionalServiceException.UpdateServiceException(model.Id);
        }
    }
}