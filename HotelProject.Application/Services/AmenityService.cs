using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . Amenity ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Users ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Logging ;

namespace HotelProject.Application.Services ;

public class AmenityService : IAmenityService
{
      private readonly IGenericRepository<Amenity, Guid> _amenityRepository;
    private readonly IGenericRepository<RoomAmenity, Guid> _roomAmenityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AmenityService> _logger;

    public AmenityService(
        IGenericRepository<Amenity, Guid> amenityRepository,
        IGenericRepository<RoomAmenity, Guid> roomAmenityRepository,
        IUnitOfWork unitOfWork,
        ILogger<AmenityService> logger)
    {
        _amenityRepository = amenityRepository;
        _roomAmenityRepository = roomAmenityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PageResult<AmenityViewModel>> GetAmenities(SearchQuery query)
    {
        var result = new PageResult<AmenityViewModel>
        {
            CurrentPage = query.PageIndex
        };

        var amenityQuery = _amenityRepository.FindAll();
        
        // Lọc theo trạng thái nếu cần
        if (query.DisplayActiveItem)
        {
            amenityQuery = amenityQuery.Where(s => s.Status == EntityStatus.Active);
        }
        
        // Lọc theo từ khóa nếu có
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            amenityQuery = amenityQuery.Where(s => s.Name.Contains(query.Keyword) || 
                                               (s.Description != null && s.Description.Contains(query.Keyword)));
        }

        // Đếm tổng số bản ghi thỏa mãn điều kiện query
        result.TotalCount = await amenityQuery.CountAsync();
        
        // Lấy dữ liệu phân trang
        var amenities = await amenityQuery
                              .OrderBy(s => s.Name)
                              .Skip(query.SkipNo)
                              .Take(query.TakeNo)
                              .Select(s => new AmenityViewModel
                              {
                                  Id = s.Id,
                                  Name = s.Name,
                                  Description = s.Description
                              })
                              .ToListAsync();

        result.Data = amenities;
        return result;
    }

    public async Task<AmenityViewModel> GetAmenityDetail(Guid amenityId)
    {
        var amenity = await _amenityRepository.FindByIdAsync(amenityId);
        if (amenity == null)
        {
            throw new AmenityException.AmenityNotFoundException(amenityId);
        }
        
        var result = new AmenityViewModel
        {
            Id = amenity.Id,
            Name = amenity.Name,
            Description = amenity.Description
        };

        return result;
    }

    public async Task<ResponseResult> CreateAmenity(AmenityCreateUpdateViewModel model, UserProfileModel currentUser)
    {
        var newAmenity = new Amenity
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            CreatedBy = currentUser.UserId,
            CreatedDate = DateTime.Now,
            Status = EntityStatus.Active
        };

        try
        {
            _amenityRepository.Add(newAmenity);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("Tạo tiện nghi mới thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo tiện nghi mới");
            throw new AmenityException.UpdateAmenityException(Guid.Empty);
        }
    }

    public async Task<ResponseResult> UpdateAmenity(Guid amenityId, AmenityCreateUpdateViewModel model, UserProfileModel currentUser)
    {
        var amenity = await _amenityRepository.FindByIdAsync(amenityId);
        if (amenity == null)
        {
            throw new AmenityException.AmenityNotFoundException(amenityId);
        }
        
        amenity.Name = model.Name;
        amenity.Description = model.Description;
        amenity.UpdatedBy = currentUser.UserId;
        amenity.UpdatedDate = DateTime.Now;

        try
        {
            _amenityRepository.Update(amenity);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("Cập nhật tiện nghi thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật tiện nghi với ID: {AmenityId}", amenityId);
            throw new AmenityException.UpdateAmenityException(amenityId);
        }
    }

    public async Task<ResponseResult> DeleteAmenity(Guid amenityId)
    {
        var amenity = await _amenityRepository.FindByIdAsync(amenityId);
        if (amenity == null)
        {
            throw new AmenityException.AmenityNotFoundException(amenityId);
        }
        
        // Kiểm tra xem tiện nghi đã được sử dụng ở phòng nào chưa
        var usedInRooms = await _roomAmenityRepository.FindAll(ra => ra.AmenityId == amenityId).AnyAsync();
        if (usedInRooms)
        {
            return ResponseResult . Fail ( "Không thể xóa tiện nghi đang được sử dụng" ) ;
        }

        try
        {
            _amenityRepository.Remove(amenity);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("Xóa tiện nghi thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa tiện nghi với ID: {AmenityId}", amenityId);
            throw new AmenityException.DeleteAmenityException(amenityId);
        }
    }

    public async Task<ResponseResult> UpdateStatus(UpdateStatusViewModel model)
    {
        var amenity = await _amenityRepository.FindByIdAsync(model.Id);
        if (amenity == null)
        {
            throw new AmenityException.AmenityNotFoundException(model.Id);
        }

        amenity.Status = model.Status;
            
        try
        {
            _amenityRepository.Update(amenity);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("Cập nhật trạng thái tiện nghi thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật trạng thái tiện nghi có ID: {AmenityId}", model.Id);
            throw new AmenityException.UpdateAmenityException(model.Id);
        }
    }
    
}