using HotelProject . Domain . Enum ;
using HotelProject . Domain . Model . Payment ;

namespace HotelProject.Domain.Abstractions.InfrastructureServices ;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPayment(PaymentRequest paymentRequest);
    Task<PaymentStatus> CheckPaymentStatus(string paymentId);
    Task<PaymentResult> RefundPayment(string paymentId, decimal amount);
}   