using HotelProject . Domain . Enum ;

namespace HotelProject.Domain.Model.Payment ;

public class PaymentRequest
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? CardNumber { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCVC { get; set; }
    public string? BankAccountNumber { get; set; }
}