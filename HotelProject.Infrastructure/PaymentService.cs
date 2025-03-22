using HotelProject . Domain . Abstractions . InfrastructureServices ;
using HotelProject . Domain . Enum ;
using HotelProject . Domain . Model . Payment ;
using Microsoft . Extensions . Logging ;

namespace HotelProject.Infrastructure ;

public class PaymentService : IPaymentService
{
     private readonly ILogger<PaymentService> _logger;

        public PaymentService(ILogger<PaymentService> logger)
        {
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPayment(PaymentRequest paymentRequest)
        {
            _logger.LogInformation("Xử lý thanh toán cho Booking ID: {BookingId}, Phương thức: {PaymentMethod}", 
                paymentRequest.BookingId, paymentRequest.PaymentMethod);

            // Mô phỏng xử lý thanh toán
            var result = new PaymentResult();
            
            switch (paymentRequest.PaymentMethod)
            {
                case PaymentMethod.Cash:
                    result.Success = true;
                    result.PaymentId = $"CASH-{Guid.NewGuid()}";
                    result.Status = PaymentStatus.Pending; 
                    result.Message = "Đang chờ xác nhận thanh toán tiền mặt";
                    break;

                case PaymentMethod.CreditCard:
                    // Mô phỏng xử lý thẻ
                    if (string.IsNullOrEmpty(paymentRequest.CardNumber))
                    {
                        result.Success = false;
                        result.Status = PaymentStatus.Failed;
                        result.Message = "Thiếu thông tin thẻ";
                    }
                    else if (paymentRequest.CardNumber == "4111111111111111") // Thẻ test
                    {
                        result.Success = true;
                        result.PaymentId = $"CC-{Guid.NewGuid()}";
                        result.Status = PaymentStatus.Paid;
                        result.Message = "Thanh toán thẻ thành công";
                    }
                    else
                    {
                        result.Success = false;
                        result.Status = PaymentStatus.Failed;
                        result.Message = "Thẻ không hợp lệ";
                    }
                    break;

                case PaymentMethod.BankTransfer:
                    result.Success = true;
                    result.PaymentId = $"BT-{Guid.NewGuid()}";
                    result.Status = PaymentStatus.Pending;
                    result.Message = "Đang chờ xác nhận chuyển khoản";
                    break;

                default:
                    result.Success = false;
                    result.Status = PaymentStatus.Failed;
                    result.Message = "Phương thức thanh toán không hỗ trợ";
                    break;
            }

            await Task.Delay(500); // Giả lập độ trễ mạng
            return result;
        }

        public async Task<PaymentStatus> CheckPaymentStatus(string paymentId)
        {
            await Task.Delay(200);
            return paymentId.StartsWith("BT-") ? PaymentStatus.Pending : PaymentStatus.Paid;
        }

        public async Task<PaymentResult> RefundPayment(string paymentId, decimal amount)
        {
            _logger.LogInformation("Hoàn tiền cho giao dịch: {PaymentId}, Số tiền: {Amount}", paymentId, amount);
            
            await Task.Delay(300);
            
            var result = new PaymentResult
            {
                Success = true,
                PaymentId = $"RF-{paymentId}",
                Status = PaymentStatus.Refunded,
                Message = $"Đã hoàn tiền: {amount:N0} VND"
            };
            
            return result;
        }  
}