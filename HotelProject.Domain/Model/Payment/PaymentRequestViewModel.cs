using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Model.Payment ;

public class PaymentRequestViewModel
{
    public Guid BookingId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? CardNumber { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCVC { get; set; }
   
}