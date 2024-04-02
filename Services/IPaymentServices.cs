using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorPayMvc.Models;

namespace RazorPayMvc.Services
{
    public interface IPaymentServices
    {
        Task<MerchantOrder> ProcessMerchantOrder(PaymentRequest paymentRequest);
        string CompleteOrderProcess(IHttpContextAccessor _httpContextAccessor);
        void RefundMoney(string paymentId);
        void CreatePayment();
    }
}