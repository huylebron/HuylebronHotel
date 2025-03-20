using HotelProject . Domain ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Domain . Entities ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Exception ;
using HotelProject . Domain . Model . Commons ;
using HotelProject . Domain . Model . Hotel ;
using HotelProject . Domain . Model . Images ;
using HotelProject . Domain . Model . RoomTypeViewModel ;
using HotelProject . Domain . Model . Users ;
using Microsoft . EntityFrameworkCore ;
using Microsoft . Extensions . Logging ;
using Newtonsoft . Json ;

namespace Demo.Application.Services ;

public class HotelService : IHotelService
{
    private readonly IGenericRepository<Hotel, Guid> _hotelRepository;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelService> _logger;
    private const string _imageFolder = "hotelImages";
    
    
    public HotelService(
        IGenericRepository<Hotel, Guid> hotelRepository,
        IUnitOfWork unitOfWork,
        IFileService fileService,
        ILogger<HotelService> logger)
    {
        _hotelRepository = hotelRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _logger = logger;
    }
    public async Task < PageResult < HotelListViewModel > > GetHotels ( SearchQuery query ) {
        var result = new PageResult<HotelListViewModel>
        {
            CurrentPage = query.PageIndex
        };

        var hotelQuery = _hotelRepository.FindAll(includeProperties: h => h.RoomTypes);
        if (query.DisplayActiveItem)
        {
            hotelQuery = hotelQuery.Where(s => s.Status == EntityStatus.Active);
        }
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            hotelQuery = hotelQuery.Where(s => s.Name.Contains(query.Keyword) || 
                                               s.Description.Contains(query.Keyword) ||
                                               s.Address.Contains(query.Keyword));
        }

        result.TotalCount = await hotelQuery.CountAsync();
        var hotels = await hotelQuery
                           .OrderBy(s => s.Name)
                           .Skip(query.SkipNo)
                           .Take(query.TakeNo)
                           .Select(s => new HotelListViewModel
                           {
                               Id = s.Id,
                               Name = s.Name,
                               Address = s.Address,
                               TotalRooms = s.RoomTypes.SelectMany(rt => rt.Rooms).Count(),
                               AvailableRooms = s.RoomTypes.SelectMany(rt => rt.Rooms)
                                                 .Count(r => r.RoomStatus == RoomStatus.Available)
                           })
                           .ToListAsync();

        // Xử lý ảnh đầu tiên cho mỗi khách sạn
        foreach (var hotel in hotels)
        {
            var hotelEntity = await _hotelRepository.FindByIdAsync(hotel.Id);
            if (!string.IsNullOrEmpty(hotelEntity.ImageJson))
            {
                var images = JsonConvert.DeserializeObject<List<ImageInEntity>>(hotelEntity.ImageJson);
                hotel.ImageUrl = images.FirstOrDefault()?.ImageUrl;
            }
        }

        result.Data = hotels;
        return result;
    }

    public async Task < HotelDetailViewModel > GetHotelDetail ( Guid hotelId ) {
        var hotel = await _hotelRepository.FindByIdAsync(hotelId, h => h.RoomTypes);
        if (hotel == null)
        {
            throw new HotelException.HotelNotFoundException(hotelId);
        }
        var result = new HotelDetailViewModel
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            RoomTypes = hotel.RoomTypes.Select(rt => new RoomTypeViewModel
            {
                Id = rt.Id,
                Name = rt.Name,
                Description = rt.Description,
                TotalRooms = rt.Rooms.Count
            }).ToList()
        };
        // Xử lý  ảnh
        if (!string.IsNullOrEmpty(hotel.ImageJson))
        {
            var images = JsonConvert.DeserializeObject<List<ImageInEntity>>(hotel.ImageJson);
            result.Images = images.Select(img => img.ImageUrl).ToList();
        }

        return result;
    }

    public async Task < ResponseResult > CreateHotel ( HotelCreateUpdateViewModel model , UserProfileModel currentUser ) {
        List<ImageInEntity> hotelImages = new List<ImageInEntity>();
            
        // Xử lý upload ảnh
        if (model.ImageFiles != null && model.ImageFiles.Any())
        {
            var uploadTasks = model.ImageFiles.Select(img => _fileService.UploadFile(img, _imageFolder));
            var uploadedImages = await Task.WhenAll(uploadTasks);
                
            hotelImages = uploadedImages.Select(img => new ImageInEntity(
                Guid.NewGuid(),
                img.FileName,
                img.FilePath
            )).ToList();
        }

        var newHotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            Address = model.Address,
            ImageJson = hotelImages.Any() ? JsonConvert.SerializeObject(hotelImages) : null,
            CreatedBy = currentUser.UserId,
            CreatedDate = DateTime.Now,
            Status = EntityStatus.Active
        };

        try
        {
            _hotelRepository.Add(newHotel);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("create hotel successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " error when create hotel");
            throw new HotelException.UpdateHotelException(Guid.Empty);
        }
    }

    public async Task < ResponseResult > UpdateHotel ( Guid hotelId , HotelCreateUpdateViewModel model , UserProfileModel currentUser ) {
        var hotel = await _hotelRepository.FindByIdAsync(hotelId);
        if (hotel == null)
        {
            throw new HotelException.HotelNotFoundException(hotelId);
        }

        hotel.Name = model.Name;
        hotel.Description = model.Description;
        hotel.Address = model.Address;
        hotel.UpdatedBy = currentUser.UserId;
        hotel.UpdatedDate = DateTime.Now;

        // Xử lý update ảnh 
        if (model.ImageFiles != null && model.ImageFiles.Any())
        {
            List<ImageInEntity> hotelImages = new List<ImageInEntity>();
                
            // Giữ lại ảnh cũ nếu có
            if (!string.IsNullOrEmpty(hotel.ImageJson))
            {
                hotelImages = JsonConvert.DeserializeObject<List<ImageInEntity>>(hotel.ImageJson);
            }
                
            // Thêm ảnh mới
            var uploadTasks = model.ImageFiles.Select(img => _fileService.UploadFile(img, _imageFolder));
            var uploadedImages = await Task.WhenAll(uploadTasks);
                
            var newImages = uploadedImages.Select(img => new ImageInEntity(
                Guid.NewGuid(),
                img.FileName,
                img.FilePath
            )).ToList();
                
            hotelImages.AddRange(newImages);
            hotel.ImageJson = JsonConvert.SerializeObject(hotelImages);
        }

        try
        {
            _hotelRepository.Update(hotel);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("update hotel successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "eror when update hotel with {HotelId}", hotelId);
            throw new HotelException.UpdateHotelException(hotelId);
        } 
    }

    public async Task < ResponseResult > DeleteHotel ( Guid hotelId ) {
        var hotel = await _hotelRepository.FindByIdAsync(hotelId);
        if (hotel == null)
        {
            throw new HotelException.HotelNotFoundException(hotelId);
        }

        try
        {
            _hotelRepository.Remove(hotel);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("delete hotel successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error delete hotel with  {HotelId}", hotelId);
            throw new HotelException.DeleteHotelException(hotelId);
        }
       
    }

    public async Task < ResponseResult > UpdateStatus ( UpdateStatusViewModel model ) {
        var hotel = await _hotelRepository.FindByIdAsync(model.Id);
        if (hotel == null)
        {
            throw new HotelException.HotelNotFoundException(model.Id);
        }

        hotel.Status = model.Status;
            
        try
        {
            _hotelRepository.Update(hotel);
            await _unitOfWork.SaveChangesAsync();
            return ResponseResult.Success("update status hotel successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error when hotel : {HotelId}", model.Id);
            throw new HotelException.UpdateHotelException(model.Id);
        }
    }
}