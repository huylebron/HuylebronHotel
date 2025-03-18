using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Model.Commons ;

public class UpdateStatusViewModel
{
    public EntityStatus Status { get; set; }
    public Guid Id { get; set; }
}