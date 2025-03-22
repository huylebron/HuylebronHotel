using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Model.Payment ;

public class PaymentResult
{
    public bool Success { get; set; }
    public string? PaymentId { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Message { get; set; }
}