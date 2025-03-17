using HotelProject . Domain . Enum ;

namespace HotelProject.Domain ;

public interface IAuditTable
{
    public DateTime? CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public EntityStatus Status { get; set; }
}